
/* ---------- bootstrap ---------- */

/* modal */
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.modal').forEach(function (model) {

            // submit
            model.querySelectorAll('.btn-modal-return').forEach(function (o) {
                o.addEventListener('click', function (event) {
                    model.returnValue = this.getAttribute("returnValue");
                    bootstrap.Modal.getInstance(model).hide();
                });
            });
        });
    });
})();


/* ---------- swiper ---------- */

/* init */
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.swiper').forEach(function (o) {

            // swiper-pagination
            var pagination = document.createElement('div');
            pagination.className = 'swiper-pagination';
            o.appendChild(pagination);

            // swiper-button-prev
            var prevButton = document.createElement('div');
            prevButton.className = 'swiper-button-prev';
            o.appendChild(prevButton);

            // swiper-button-next
            var nextButton = document.createElement('div');
            nextButton.className = 'swiper-button-next';
            o.appendChild(nextButton);

            // swiper-button-close
            var closeButton = document.createElement('div');
            closeButton.className = 'swiper-button-close';
            closeButton.innerHTML = '&#215;';
            closeButton.addEventListener('click', function () {
                this.parentNode.classList.toggle("swiper-slide-fullscreen");
            });
            o.appendChild(closeButton);

            // swiper
            var swiper = new Swiper(o, {
                loop: true,
                spaceBetween: 16,
                slidesPerView: "auto",
                pagination: {
                    el: ".swiper-pagination",
                    clickable: true
                },
                navigation: {
                    nextEl: ".swiper-button-next",
                    prevEl: ".swiper-button-prev"
                }
            });
            swiper.on('click', function (e) {
                this.el.classList.toggle("swiper-slide-fullscreen");
            });
        });
    });
})();


/* ---------- pikaday ---------- */

/* init */
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.pikaday').forEach(function (o) {

            // fields
            o.pikaday = null;

            // methods
            o.show = function (options) {

                // destroy
                if (o.pikaday) {
                    o.pikaday.destroy();
                    o.pikaday = null;
                }

                // options
                options = options || {};
                options.field = o;
                options.container = o;
                options.bound = false;
                options.firstDay = 0;
                options.showDaysInNextAndPreviousMonths = true;
                options.enableSelectionDaysInNextAndPreviousMonths = true;
                options.format = 'YYYY/MM/DD';
                options.i18n = {
                    previousMonth: '上個月',
                    nextMonth: '下個月',
                    months: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                    weekdays: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
                    weekdaysShort: ['日', '一', '二', '三', '四', '五', '六']
                };
                options.onSelect= function (date) {
                    
                    // event
                    var event = new CustomEvent('select', {
                        detail: date
                    });

                    // dispatch
                    o.dispatchEvent(event);
                }

                // pikaday
                o.pikaday = new Pikaday(options);
            };    
        });
    });
})();