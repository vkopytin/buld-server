// set up ========================
import * as express from "express";
import * as morgan from "morgan"; // log requests to the console (express4)
import * as path from "path"; // normalize the paths : http://stackoverflow.com/questions/9756567/do-you-need-to-use-path-join-in-node-js
import * as bodyParser from "body-parser"; // pull information from HTML POST (express4)
import * as methodOverride from "method-override"; // simulate DELETE and PUT (express4)
import * as helmet from "helmet"; // Security
import * as compression from "compression";
import * as routes from "./routes";


export class App {

    protected app: express.Application;

    constructor(NODE_ENV: string = 'development', PORT: string = '8080') {

        /**
         * Setting environment for development|production
         */
        process.env.NODE_ENV = process.env.NODE_ENV || NODE_ENV;

        /**
         * Setting port number
         */
        process.env.PORT = process.env.PORT || PORT;


        /**
         * Create our app w/ express
         */
        this.app = express();
        this.app.use(helmet());

        if (NODE_ENV === 'development') {
            this.app.use(express.static(path.join(process.cwd(), 'static')));
            this.app.use('/bower_components', express.static(path.join(process.cwd(), 'bower_components'))); // set the static files location of bower_components
            this.app.use(morgan('dev'));  // log every request to the console
        } else {
            this.app.use(compression());
            this.app.use(express.static(path.join(process.cwd(), 'dist'), { maxAge: '7d' })); // set the static files location /public/img will be /img for users
        }

        this.app.use(bodyParser.urlencoded({ 'extended': true })); // parse application/x-www-form-urlencoded
        this.app.use(bodyParser.json()); // parse application/json
        this.app.use(bodyParser.json({ type: 'application/vnd.api+json' })); // parse application/vnd.api+json as json
        this.app.use(methodOverride());


        /**
         * Setting routes
       */
        let __routes = new routes.Routes(process.env.NODE_ENV);
        __routes.paths(this.app);

        /**
         * START the server
       */
        this.app.listen(process.env.PORT, function () {
            console.log('The server is running in port localhost: ', process.env.PORT);
        });

    }

}

new App