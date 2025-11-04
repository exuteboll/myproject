document.addEventListener('DOMContentLoaded', function () {
    // Функция для открытия/закрытия модального окна
    function toggleModal() {
        const modal = document.querySelector(".container-login-registration");
        if (modal.style.display === "none" || modal.style.display === "") {
            modal.style.display = "flex"; // Исправлено на flex
        } else {
            modal.style.display = "none";
        }
    }

    // Обработчик для кнопки в шапке
    const headerButton = document.getElementById("click-to-hide");
    if (headerButton) {
        headerButton.addEventListener("click", toggleModal);
    }

    // Обработчик для закрытия по клику на overlay
    const overlay = document.querySelector(".overlay");
    if (overlay) {
        overlay.addEventListener("click", toggleModal);
    }

    // Логика переключения между формами
    const signInBtn = document.querySelector('.signin-btn');
    const signUpBtn = document.querySelector('.signup-btn');
    const formBox = document.querySelector('.form-box');
    const block = document.querySelector('.block');

    if (signInBtn && signUpBtn) {
        signUpBtn.addEventListener('click', function () {
            formBox.classList.add('active');
            block.classList.add('active');
        });

        signInBtn.addEventListener('click', function () {
            formBox.classList.remove('active');
            block.classList.remove('active');
        });
    }
});