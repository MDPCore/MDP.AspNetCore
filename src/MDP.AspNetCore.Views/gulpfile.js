// variables
var libraryList = [
    //{ name: "bootstrap", path: "./node_modules/bootstrap/dist/**/", src: ["*.css*", "*.js*"] },
    //{ name: "@popperjs", path: "./node_modules/@popperjs/core/dist/**/", src: ["*.js*"] },
    //{ name: "@splidejs", path: "./node_modules/@splidejs/splide/dist/**/", src: ["*.css*", "*.js*"] }
];

// require
var gulp = require("gulp");
var rimraf = require("rimraf");
var cssnano = require('cssnano');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var postcss = require('gulp-postcss');
var postcssImport = require('postcss-import');

// task
gulp.task("lib-clean", function (cb) {
	
	// delete
    rimraf("./wwwroot/lib", cb);
});

gulp.task("lib-copy", function (done) {
	
	// copy
    libraryList.forEach(function (library) {
        library.src.forEach(function (src) {
            gulp.src(library.path + src).pipe(gulp.dest("./wwwroot/lib/" + library.name));
        });
    });
	
	// done
    done();
});

gulp.task("lib-build", function (done) {

    // css
    gulp.src('./css/*.css')
        .pipe(postcss([
            postcssImport(),
            cssnano()
        ]))
        .pipe(concat('mdp-aspnetcore-views.min.css'))
        .pipe(gulp.dest('./wwwroot/lib/mdp-aspnetcore-views/css'));

    // js
    gulp.src('./js/*.js')
        .pipe(concat('mdp-aspnetcore-views.min.js'))
        .pipe(uglify()) 
        .pipe(gulp.dest('./wwwroot/lib/mdp-aspnetcore-views/js'));

    // img
    gulp.src('./img/**/*')
        .pipe(gulp.dest('./wwwroot/lib/mdp-aspnetcore-views/img'));

    // done
    done();
});

gulp.task("publish", gulp.series("lib-clean", "lib-copy", "lib-build"));