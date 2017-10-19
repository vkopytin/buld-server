import git = require('simple-git');

class Git {
    impl: git = null

    constructor (path) {
        this.impl = git(path);
    }

    clone (repositoryUrl) {
        var res = new Promise((resolve, reject) => {
            this.impl.clone(repositoryUrl, '', [], (err, res) => {
                err && reject(err);
                !err && resolve(res);
            });
        });

        return res;
    }

    checkout(branch) {
        var res = new Promise((resolve, reject) => {
            this.impl.checkout([branch], (err, res) => {
                err && reject(err);
                !err && resolve(res);
            });
        });

        return res;
    }

    pull() {
        var res = new Promise((resolve, reject) => {
            this.impl.pull((err, res) => {
                err && reject(err);
                !err && resolve(res);
            });
        });

        return res;
    }

    static clone (path, repositoryUrl) {
        var res = new Promise((resolve, reject) => {
            git(path).clone(repositoryUrl, '', [], (err, res) => {
                err && reject(err);
                !err && resolve(res);
            });
        });

        return res;
    }
};

export { Git };