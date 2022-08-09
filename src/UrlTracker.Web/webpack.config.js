const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const RemoveEmptyScriptsPlugin = require('webpack-remove-empty-scripts');
const magicImporter = require('node-sass-magic-importer');
const CopyPlugin = require('copy-webpack-plugin');

const config = {
    entry: {
        script: './frontend/js/index.ts',
        style: './frontend/scss/style.scss'
    },
    output: {
        path: path.resolve(__dirname, 'content/App_Plugins/UrlTracker'),
        filename: '[name].bundle.js',
        publicPath: '/content/App_Plugins/UrlTracker/',
        assetModuleFilename: '[path][name][ext]'
    },
    module: {
        rules: [
            {
                test: /\.ts(x)?$/,
                loader: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    {
                        loader: 'sass-loader',
                        options: {
                            webpackImporter: false,
                            sassOptions: {
                                importer: [
                                    magicImporter()
                                ],
                                quietDeps: true
                            }
                        }
                    }
                ]
            }
        ]
    },
    resolve: {
        extensions: [
            '.tsx',
            '.ts',
            '.js'
        ]
    },
    plugins: [
        new MiniCssExtractPlugin(),
        new RemoveEmptyScriptsPlugin(),
        new CopyPlugin({
            patterns: [
                { from: '**/*.{html,svg}', to: '.', context: 'frontend/js' },
                { from: 'lang/**/*.*', to: '.', context: 'frontend' },
                { from: 'assets/**/*.*', to: '.', context: 'frontend' }
            ]
        })
    ]
};

module.exports = config;