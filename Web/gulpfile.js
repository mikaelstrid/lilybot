/// <binding AfterBuild='min:scss:site' />
'use strict';

var gulp = require('gulp'),
    rimraf = require('rimraf'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-cssmin'),
    uglify = require('gulp-uglify'),
    filter = require('gulp-filter'),
    rename = require('gulp-rename'),
    sass = require('gulp-sass');

gulp.task("default", ["watch"]);


// === SASS ===
gulp.task('min:scss:site', function () {
    return gulp.src('./scss/site.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('./wwwroot/assets/css'))
        .pipe(rename('site.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('./wwwroot/assets/css'));
});


// === WATCH ===
gulp.task('watch', function () {
    gulp.watch('./scss/**/*.scss', ['min:scss:site']);
});