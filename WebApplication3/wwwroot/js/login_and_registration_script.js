document.addEventListener('DOMContentLoaded', function () {
    // Функция для открытия/закрытия модального окна

    function toggleModal() {
        const modal = document.querySelector(".container-login-registration");
        if (modal.style.display === "none" || modal.style.display === "") {
            modal.style.display = "flex";
            document.body.style.overflow = 'hidden';
            document.body.style.position = 'fixed'; // Фиксируем позицию на iOS
            document.body.style.width = '100%';
        } else {
            modal.style.display = "none";
            document.body.style.overflow = '';
            document.body.style.position = '';
            document.body.style.width = '';
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
    /*
    const closeButton = document.getElementById("close-modal-btn");
    if (closeButton) {
        closeButton.addEventListener("click", toggleModal);
        console.log("Кнопка закрытия найдена и обработана");
    } */

    // Логика переключения между формами
    const signInBtn = document.querySelector('.signin-btn');
    const signUpBtn = document.querySelector('.signup-btn');
    const formBox = document.querySelector('.form-box');
    const block = document.querySelector('.block');


    const form_btn_signin = document.querySelector('.form-btn');
    const form_btn_signup = document.querySelector('.form_btn_signup');

    if (form_btn_signin) {
        form_btn_signin.addEventListener('click', function () {
            const requestURL = '/Home/Login';

            const errorContainer = document.getElementById('error-messages-singin');

            const form = {
                email: document.querySelector("#signin_email input"),
                password: document.querySelector("#signin_password input")
            }

            const body = {
                email: form.email.value,
                password: form.password.value
            }

            sendRequest('POST', requestURL, body)
                .then(data => {
                    cleaningAndClosingForm(form, errorContainer); // твой код
                    console.log('Успешный ответ:', data);
                    location.reload(); // твой код
                })
                .catch(err => {
                    displayErrors(err, errorContainer);
                    console.log(err);
                });
        });
    }
    function cleaningAndClosingForm(form, errorContainer) {
        errorContainer.innerHTML = "";
        for (const key in form) {
            if (form.hasOwnProperty(key)) {
                form[key].value = ""; // Сброс значений полей формы
            }
        }
        hiddenOpen_CloseClick();
    }
    function displayErrors(errors, errorContainer) {
        errorContainer.innerHTML = ''; // Очистить предыдущие ошибки
        errors.forEach(error => {
            const errorMessage = document.createElement('div');
            errorMessage.classList.add('error');
            errorMessage.textContent = error;
            errorContainer.appendChild(errorMessage);
        });
    }
    if (form_btn_signup) {
        form_btn_signup.addEventListener('click', function () {
            const requestURL = '/Home/Register';
            const errorContainer = document.getElementById('error-messages-singup');
            const form = {
                login: document.getElementById("signup_login"),
                email: document.getElementById("signup_email"),
                password: document.getElementById("signup_password"),
                passwordConfirm: document.getElementById("signup_confirm_password"),
            }

            const body = {
                login: form.login.value,
                email: form.email.value,
                password: form.password.value,
                passwordConfirm: form.passwordConfirm.value,
            }

            sendRequest('POST', requestURL, body)
                .then(data => {
                    cleaningAndClosingForm(form, errorContainer);
                    console.log('Успешный ответ:', data);
                    location.reload();
                })
                .catch(err => {
                    displayErrors(err, errorContainer);
                    console.log(err);
                });
        });
    }

    function sendRequest(method, url, body = null) {
        const headers = {
            'Content-Type': 'application/json'
        }
        return fetch(url, {
            method: method,
            body: JSON.stringify(body),
            headers: headers
        }).then(response => {  // ЦЕПОЧКА then ПРЯМО В ФУНКЦИИ
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw errorData;
                });
            }
            return response.json();
        });
    }

    

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

    /*document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
            const modal = document.querySelector(".container-login-registration");
            if (modal && modal.style.display === "flex") {
                toggleModal();
            }
        }
    }); */

    /*Для мобилок */
    document.addEventListener('DOMContentLoaded', function () {
        const sideMenuButton = document.getElementById("side-menu-button-click-to-hide");
        if (sideMenuButton) {
            sideMenuButton.addEventListener("click", toggleModal); // Используем существующую функцию
        }
    });
    document.addEventListener('click', function (e) {
        if (e.target.id === 'side-menu-button-click-to-hide' ||
            e.target.closest('#side-menu-button-click-to-hide')) {
            e.preventDefault();
            console.log('Delegated handler caught the click!');
            toggleModal();
        }
    });

});

