import * as path from 'path'; // normalize the paths : http://stackoverflow.com/questions/9756567/do-you-need-to-use-path-join-in-node-js
import express = require('express');
import git = require('simple-git');
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

function $when(value) {
    if (value && ('then' in value)) {
        return value;
    }
    return {
        then: fn => fn(value)
    };
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

const yarnExec = (cmd: any[], then, options?) => {
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

const buildChannel = (branch, build, then) => {
    ifExists('./tmp/runnereditor',
        () => { },
        () => git('./tmp').clone('git@github.com:RebelMouseTeam/runnereditor.git'),
        () => git('./tmp/runnereditor')
    )
    //.clone('git@github.com:RebelMouseTeam/runnereditor.git')
    .then(() => {
        git('./tmp/runnereditor').checkout([branch], () =>
            git('./tmp/runnereditor').pull().then(() =>
                yarnExec(['install'], (out, out2) => writeBuildLog(out + out2, () =>
                    writeEnv(() =>
                        yarnExec([build], (out, out2) => writeBuildLog(out + out2, () => {
                            then(out, out2);
                        }))
                    ))
                )
            )
        );
    });
}

export class Routes {

    protected basePath: string;

    //protected api: database.API;

    constructor(NODE_ENV: string) {

        switch (NODE_ENV) {
            case 'production':
                this.basePath = '/build/dist';
                break;
            case 'development':
                this.basePath = '/build/static';
                break;
        }

    }

    defaultRoute(req: express.Request, res: express.Response) {
        res.sendFile('index.html', { root: path.join(process.cwd(), this.basePath) });
    }

    paths(app: express.Application) {

        app.get('/api/deploy/:branch', (req: express.Request, res: express.Response, next: express.NextFunction) => {
            var branch = req.params.branch;
            var buildCode = req.query.code || '';
            var build = ['build', (req.query.build || 'alpha')].join('-');
            var buildName = './tmp/build-' + buildCode + '.log';

            if (tasks.length === 0) {
                ifExists(
                    buildName,
                    content => res.send({
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
                        });
                    },
                    () => { }
                );
                res.send({
                    build: 'queued...',
                });
            } else {
                ifExists('./tmp/build.log',
                    (content) => res.send({
                        test: 'build is in progress',
                        details: content
                    }),
                    () => { },
                    () => { }
                );
            }
        });

        app.get('/', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

        app.get('*', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

    }

}