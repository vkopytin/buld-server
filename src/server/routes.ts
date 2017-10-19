import _ = require('underscore');
import * as path from 'path'; // normalize the paths : http://stackoverflow.com/questions/9756567/do-you-need-to-use-path-join-in-node-js
import express = require('express');
import controllers = require('./controllers');


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

    routes = [
        { path: '/:controller/:action/:branch', defaults: { controller: 'Home', action: 'index', branch: 'alpha_channel' } },
        { path: '/:controller/:action', defaults: { controller: 'Home', action: 'index', branch: 'alpha_channel' } }
    ]

    findController(controllers, name) {
        return _.find(controllers, (v, k) => k.toLowerCase() === name.toLowerCase());
    }

    paths(app: express.Application) {

        this.routes.forEach(routeMap => {
            var { path, defaults } = routeMap;
            app.get(path, (req: express.Request, res: express.Response, next: express.NextFunction) => {
                var { controller, action } = _.extend({}, defaults, _.pick(req.params, 'controller', 'action'));
                var ControllerCtor = this.findController(controllers, controller);
                var params = _.defaults(req.params, defaults);
                var controller = new ControllerCtor();
                var result = controller[action].call(controller, _.extend({}, params, req.query));
                (async (result) => res.send(await result))(result);
            });
        });

        app.get('/', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

        app.get('*', (req: express.Request, res: express.Response) => {
          this.defaultRoute(req, res);
        });

    }

}