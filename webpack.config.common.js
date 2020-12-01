const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const mode = process.env.NODE_ENV || "production";

module.exports = {
    entry: {
        app: "./web/ts/app.ts"
    },
    resolve: {
        extensions: [".ts", ".js", ".json"],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css",
        }),
        new HtmlWebpackPlugin({
            minify: false
        }),
        new CleanWebpackPlugin({
            cleanStaleWebpackAssets: mode === "production"
        })
    ],
    output: {
        path: path.resolve(__dirname, "wwwroot/dist"),
        filename: "[name].bundle.js",
        library: "JsInterop"
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            },
            {
                test: /\.(s[ac]ss|css)$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "postcss-loader",
                    "sass-loader"
                ]
            },
        ]
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                defaultVendors: {
                    name: "vendors",
                    chunks: "all",
                    test: /[\\/]node_modules[\\/]/
                }
            }
        }
    }
};