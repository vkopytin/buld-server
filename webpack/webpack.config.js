const path = require('path')
const webpack = require('webpack')
const CleanWebpackPlugin = require('clean-webpack-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')

var PUBLIC_DIR = 'build';

const config = {
  devtool: 'eval', ///'source-map',
  target: 'node',
  entry: {
    app: [path.join(__dirname, '../src/server/main.ts')],
  },
  output: {
    path: path.resolve(path.join(__dirname, '../', PUBLIC_DIR)),
    filename: '[name].js'
  },
  resolve: {
    modules: ['src', 'node_modules'],
    extensions: ['.ts', '.tsx', '.js', '.jsx', '.json']
  },
  plugins: [
    new webpack.NamedModulesPlugin(),
    new CleanWebpackPlugin(['../' + PUBLIC_DIR], {
      allowExternal: true
    }),
    new CopyWebpackPlugin([
      // {output}/file.txt
      { from: 'src/static', to: 'static' }
    ])
  ],
  module: {
    rules: [{
      test: /\.tsx?$/, loaders: ['babel-loader', 'ts-loader'], exclude: /node_modules/
    }, {
      test: /\.jsx?$/, loader: 'babel-loader', exclude: /node_modules/
    }, {
      test: /\.mustache$/, loader: 'mustache-loader'
    }, {
      test: /\.html$/, loader: 'file-loader?name=[path][name].[ext]'
    }]
  }
}

module.exports = config
