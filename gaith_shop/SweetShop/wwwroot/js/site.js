// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener('scroll', function () {
    var header = document.querySelector('header');
    if (window.scrollY > 150) {
        document.body.classList.add('sticky-mode');
    } else {
        document.body.classList.remove('sticky-mode');
    }
});
