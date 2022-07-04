const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const RemoveEmptyScriptsPlugin = require('webpack-remove-empty-scripts');
const magicImporter = require('node-sass-magic-importer');
const CopyPlugin = require('copy-webpack-plugin');

function config(env, argv) {

    let outputPath;
    if (argv.mode === "development") {
        outputPath = '../../test/UrlTracker.Resources.Website/App_Plugins/UrlTracker';
    }
    else {
        outputPath = '../../App_Plugins/UrlTracker';
    }

    return {
        entry: {
            script: './src/js/index.ts',
            style: './src/scss/style.scss'
        },
        output: {
            path: path.resolve(__dirname, outputPath),
            filename: '[name].bundle.js',
            publicPath: '/' + outputPath + '/',
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
                    { from: '**/*.{html,svg}', to: '.', context: 'src/js' },
                    { from: 'package.manifest', to: '.', context: 'src' },
                    { from: 'lang/**/*.*', to: '.', context: 'src' },
                    { from: 'assets/**/*.*', to: '.', context: 'src' }
                ]
            })
        ]
    };
};

module.exports = config;