import * as path from 'path'; // normalize the paths : http://stackoverflow.com/questions/9756567/do-you-need-to-use-path-join-in-node-js
import express = require('express');
import git = require('simple-git');
import ChildProcess = require('child_process');
import fs = require('fs');

function statPath(path) {
    try {
        return fs.statSync(path);
    } catch (ex) {}
    return false;
}

const ifExists = (path, then, _else, next) => {
    var stat = statPath(path);
    var res;
    if (stat && (stat.isDirectory() || stat.isFile())) {
        res = then(stat.isFile() ? fs.readFileSync(path, 'utf8'): '');
    } else {
        res = _else();
    }
    return next(res);
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
        stdOut.push(buffer);
     });
     spawned.stderr.on('data', function (buffer) {
        stdErr.push(buffer);
     });

     spawned.on('error', function (err) {
        stdErr.push(new Buffer(err.stack, 'ascii'));
     });
     spawned.on('close', function (exitCode, exitSignal) {
         if (exitCode && stdErr.length) {
             then.call(this, Buffer.concat(stdErr).toString('utf-8'));
         }
         else {
             if (options.concatStdErr) {
                 [].push.apply(stdOut, stdErr);
             }

             var stdOutput = Buffer.concat(stdOut);
             if (options.format !== 'buffer') {
                 var strStdOutput = stdOutput.toString(options.format || 'utf-8');
             }

             then.call(this, null, strStdOutput);
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
        git('./tmp/runnereditor').checkout([branch],
            yarnExec(['install'], (out, out2) => writeBuildLog(out + out2, () =>
                writeEnv(() =>
                    yarnExec([build], (out, out2) => writeBuildLog(out + out2, () => {
                        then(out, out2);
                    }))
                ))
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
            var branch = req.query.branch;
            var build = ['build', (req.query.build || 'alpha')].join('-');

            ifExists('./tmp/build.log',
                (content) => res.send({
                    test: 'build is in progress',
                details: content
                }), () => writeBuildLog('starting...', () =>
                    buildChannel(branch, build, (out, out2) => {
                        fs.unlinkSync('./tmp/build.log');
                        res.send({
                            test: 'test',
                            out: out,
                            out2: out2
                        });
                    })
                ),
            () => { }
            );
        });

        app.get('/', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

        app.get('*', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

    }

}