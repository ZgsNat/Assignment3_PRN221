// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer")
    .build();

connection.on("LoadAppUsers", function () {
    location.href = '/AppUsers'
});
connection.on("LoadPosts", function () {
    location.href = '/Posts'
});
connection.start().catch(function (err) {
    return console.error(err.toString());
});