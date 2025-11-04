/*document.addEventListener('DOMContentLoaded', function () {
    window.addEventListener('scroll', function () {
        var heade = document.getElementById('header-top')
        var scrollTop = window.ScrollY;
        var maxScroll = 250;

        var opacity = Math.min(scrollTop / maxScroll, 1);
        header.style.backgroundColor = `rgba(0, 0, 139, ${opacity})`;
    });
});
*/
document.addEventListener('DOMContentLoaded', function () {
    window.addEventListener('scroll', function () {
        var header = document.getElementById('header-top');
        var scrollTop = window.scrollY;
        var maxScroll = 250;

        if (scrollTop > maxScroll) {
            /* header.style.backgroundColor = 'orange'; */
            header.style.backgroundColor = 'transparent';
        } else {
            /* header.style.backgroundColor = 'transparent'; */
            header.style.backgroundColor = 'orange';
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    let currentSlide = 0;
    const cardWrapper = document.querySelector('.card-wrapper');
    const cards = document.querySelectorAll('.card');
    const cardsPerRow = 3;
    const totalSlides = Math.ceil(cards.length / cardsPerRow);

    function next() {
        if (currentSlide < totalSlides - 1) {
            currentSlide++;
            cardWrapper.style.transform = `translateX(-${currentSlide * 100}%)`;
        }
    }

    function prev() {
        if (currentSlide > 0) {
            currentSlide--;
            cardWrapper.style.transform = `translateX(-${currentSlide * 100}%)`;
        }
    }

    document.querySelector('.arrow.right').addEventListener('click', next);
    document.querySelector('.arrow.left').addEventListener('click', prev);
});
