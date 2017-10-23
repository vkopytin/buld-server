import git = require('simple-git');
import { Git } from '../git';
import ChildProcess = require('child_process');
import fs = require('fs');
import stripAnsi = require('strip-ansi');
import queue = require('queue');

const buildLogName = './tmp/build.log';

var tasks = queue();

function stripStdOut (str) {
    var res = stripAnsi(str);

    return ('' + res).replace(/[\b]+/g, '');
}

const writeEnv = async () => {
    return await fsWriteFile('./tmp/runnereditor/.env', `NODE_ENV=development
APP_IP=0.0.0.0
APP_PORT=3000
BASE_URL=http://0.0.0.0:3000
CDN=//static.rbl.ms/static/
PROXY_TARGET=https://vkopytin-secure.rebelmouse.com/
SESSIONID=lpylbgxlz3jprebjngrxf29lamhn8f96
`);
}

const writeBuildLog = async (content) => {
    return await fsWriteFile(buildLogName, content);  
}

function fsExists(path: string) {
    return new Promise((resolve, reject) => {
        fs.stat(path, (err, stats) => {
            if (err) {
                return resolve(false);
            }
            if (stats && (stats.isDirectory() || stats.isFile())) {
                return resolve(true);
            }
            return resolve(false);
        });
    });
}

const fsWriteFile = async (fullName, content) => {
    return new Promise((resolve, reject) => {
        fs.writeFile(fullName, content, 'ascii', (err) => {
            err ? reject(err) : resolve(true);
        });
    });
}

const fsRename = async (oldPath, newPath) => {
    return new Promise((resolve, reject) => {
        fs.rename(oldPath, newPath, err => {
            err ? reject(err) : resolve(true);
        });
    })
}

const fsReadFile = async (path) => {
    return new Promise((resolve, reject) => {
        fs.readFile(path, 'utf8', (err, data: string) => {
            err ? reject(err) : resolve(data);
        });
    });
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
        fs.appendFileSync(buildLogName, stripStdOut(buffer.toString()));
        stdOut.push(buffer);
     });
    spawned.stderr.on('data', function (buffer) {
        fs.appendFileSync(buildLogName, stripStdOut(buffer.toString()));
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

function yarn(cmd, options?) {
    return new Promise((resolve, reject) => {
        yarnExec(cmd, (err, res) => {
            if (err) {
                reject(err);
            }
            resolve(res);
        }, options);
    });
}

const buildChannel = async (branch, build) => {
    if (!await fsExists('./tmp/runnereditor')) {
        await Git.clone('./tmp', 'git@github.com:RebelMouseTeam/runnereditor.git');
    }

    var _git = new Git('./tmp/runnereditor');
    await _git.checkout([branch]);
    await _git.pull();

    var out = await yarn(['install']);
    await writeBuildLog(out);

    await writeEnv();

    var out = await yarn([build]);
    await writeBuildLog(out);

    return out;
}

const copyFilesToFolder = (from, to) => {
    
}
class Home {
    request: any

    constructor(req) {
        this.request = req;
    }

    index (params) {
        return {
            test: 'test'
        };
    }

    async deploy ({ branch, build, code }) {
        var buildCode = code || '';
        build = ['build', (build || 'alpha')].join('-');
        var buildName = './tmp/build-' + buildCode + '.log';
        if (tasks.length === 0) {
            if (await fsExists(buildName)) {
                return {
                    test: 'build is in progress',
                    details: await fsReadFile(buildName)
                };
            } else {
                await fsWriteFile(buildName, 'starting...\n');
                tasks.push(async cb => {
                    await writeBuildLog('starting...');
                    var out = await buildChannel(branch, build);
                    if (await fsExists(buildName)) {
                        fs.unlinkSync(buildName);
                    }
                    await fsRename(buildLogName, buildName);
                    fs.appendFileSync(buildName, '\n\n\n...done!!!');
                    cb();
                    tasks.end();
                });
                tasks.start();
                return {
                    build: 'queued...',
                };
            }
        } else {
            if (await fsExists(buildLogName)) {
                return {
                    test: 'build is in progress',
                    details: await fsReadFile(buildLogName)
                };
            }
        }
    }
}

export { Home };