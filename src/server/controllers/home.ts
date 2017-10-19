import git = require('simple-git');
import { Git } from '../git';
import ChildProcess = require('child_process');
import fs = require('fs');
import stripAnsi = require('strip-ansi');
import queue = require('queue');


var tasks = queue();

function stripStdOut (str) {
    var res = stripAnsi(str);

    return ('' + res).replace(/[\b]+/g, '');
}

function statPath(path) {
    try {
        return fs.statSync(path);
    } catch (ex) {}
    return false;
}

async function $when(value) {
    return await value;
}

const ifExists = (path, then, _else, next) => {
    var stat = statPath(path);
    var res;
    if (stat && (stat.isDirectory() || stat.isFile())) {
        res = then(stat.isFile() ? fs.readFileSync(path, 'utf8'): '');
    } else {
        res = _else();
    }
    return $when(res).then(r => next(r));
}

const writeEnv = callback => {
    fs.writeFile('./tmp/runnereditor/.env', `NODE_ENV=development
APP_IP=0.0.0.0
APP_PORT=3000
BASE_URL=http://0.0.0.0:3000
CDN=//static.rbl.ms/static/
PROXY_TARGET=https://vkopytin-secure.rebelmouse.com/
SESSIONID=lpylbgxlz3jprebjngrxf29lamhn8f96
`, 'ascii', callback);    
}

const writeBuildLog = (content, callback) => {
    fs.writeFile('./tmp/build.log', content, 'ascii', callback);  
}

const yarnExec = (cmd: string[], then, options?) => {
    var _command = 'yarn';
    var stdOut = [];
    var stdErr = [];
    var spawned = ChildProcess.spawn(_command, cmd.slice(0), {
        cwd: './tmp/runnereditor'
    });
    options = options || {
        concatStdErr: true
    };
    
    spawned.stdout.on('data', function (buffer) {
        fs.appendFileSync('./tmp/build.log', stripStdOut(buffer.toString()));
        stdOut.push(buffer);
     });
    spawned.stderr.on('data', function (buffer) {
        fs.appendFileSync('./tmp/build.log', stripStdOut(buffer.toString()));
        stdErr.push(buffer);
     });

     spawned.on('error', function (err) {
        stdErr.push(new Buffer(err.stack, 'ascii'));
     });
     spawned.on('close', function (exitCode, exitSignal) {
         if (exitCode && stdErr.length) {
             then.call(this, stripStdOut(Buffer.concat(stdErr).toString('utf-8')));
         }
         else {
             if (options.concatStdErr) {
                 [].push.apply(stdOut, stdErr);
             }

             var stdOutput = Buffer.concat(stdOut);
             if (options.format !== 'buffer') {
                 var strStdOutput = stdOutput.toString(options.format || 'utf-8');
             }

             then.call(this, null, stripStdOut(strStdOutput));
         }
     });
}

const yarn = (cmd, options?) => {
    return new Promise((resolve, reject) => {
        yarnExec(cmd, (err, res) => {
            err && reject(err);
            !err && resolve(res);
        }, options);
    });
}    

const buildChannel = async (branch, build, then) => {
    await ifExists('./tmp/runnereditor',
        () => { },
        () => Git.clone('./tmp', 'git@github.com:RebelMouseTeam/runnereditor.git'),
        () => { }
    );

    var _git = new Git('./tmp/runnereditor');
    await _git.checkout([branch]);
    await _git.pull();
    var out = await yarn(['install']);
    writeBuildLog(out, () =>
        writeEnv(() =>
            yarnExec([build], (out, out2) => writeBuildLog(out + out2, () => {
                then(out, out2);
            }))
        ));
}


class Home {
    index (params) {
        return {
            test: 'test'
        };
    }

    deploy (params) {
        var { branch, build, code } = params;
        var buildCode = code || '';
        build = ['build', (build || 'alpha')].join('-');
        var buildName = './tmp/build-' + buildCode + '.log';
        return new Promise((resolve, reject) => {
            if (tasks.length === 0) {
                ifExists(
                    buildName,
                    content => resolve({
                        test: 'build is in progress',
                        details: content
                    }),
                    () => {
                        fs.writeFile(buildName, 'starting...\n', 'ascii', () => {
                            tasks.push(cb => {
                                writeBuildLog('starting...', () =>
                                    buildChannel(branch, build, (out, out2) => {
                                        ifExists(
                                            buildName,
                                            () => fs.unlinkSync(buildName),
                                            () => { },
                                            () => {
                                                fs.rename('./tmp/build.log', buildName, () => {
                                                    fs.appendFileSync(buildName, '\n\n\n...done!!!');
                                                    cb();
                                                    tasks.end();
                                                });
                                            }
                                        );
                                    })
                                );
                            });
                            tasks.start();
                            resolve({
                                build: 'queued...',
                            });
                        });
                    },
                    () => { }
                );
            } else {
                ifExists('./tmp/build.log',
                    (content) => resolve({
                        test: 'build is in progress',
                        details: content
                    }),
                    () => { },
                    () => { }
                );
            }
        });
    }
}

export { Home };