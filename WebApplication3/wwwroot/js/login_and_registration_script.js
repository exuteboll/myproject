    document.addEventListener('DOMContentLoaded', function () {
        // Функция для открытия/закрытия модального окна
        function toggleModal() {
            const modal = document.querySelector(".container-login-registration");
            if (modal.style.display === "none" || modal.style.display === "") {
                modal.style.display = "flex";
                document.body.style.overflow = 'hidden';
                document.body.style.position = 'fixed';
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

        // Логика переключения между формами
        const signInBtn = document.querySelector('.signin-btn');
        const signUpBtn = document.querySelector('.signup-btn');
        const formBox = document.querySelector('.form-box');
        const block = document.querySelector('.block');

        const form_btn_signin = document.querySelector('.form-btn');
        const form_btn_signup = document.querySelector('.form_btn_signup');

        // Обработчик для входа
        if (form_btn_signin) {
            form_btn_signin.addEventListener('click', function () {
                const requestURL = '/Home/Login';
                const errorContainer = document.getElementById('error-messages-singin');

                const emailInput = document.querySelector("#signin_email");
                const passwordInput = document.querySelector("#signin_password");

                const body = {
                    email: emailInput.value,
                    password: passwordInput.value
                }

                sendRequest('POST', requestURL, body)
                    .then(data => {
                        if (data.success) {
                            cleaningAndClosingForm({ email: emailInput, password: passwordInput }, errorContainer);
                            console.log('Успешный вход:', data);
                            location.reload();
                        } else {
                            displayErrors([data.error || 'Ошибка входа'], errorContainer);
                        }
                    })
                    .catch(err => {
                        if (Array.isArray(err)) {
                            displayErrors(err, errorContainer);
                        } else if (err.errors) {
                            displayErrors(err.errors, errorContainer);
                        } else {
                            displayErrors([err.error || 'Ошибка сервера'], errorContainer);
                        }
                        console.log('Ошибка входа:', err);
                    });
            });
        }

        // Обработчик для регистрации
        if (form_btn_signup) {
            form_btn_signup.addEventListener('click', function () {
                const requestURL = '/Home/Register';
                const errorContainer = document.getElementById('error-messages-singup');

                const loginInput = document.getElementById("signup_login");
                const emailInput = document.getElementById("signup_email");
                const passwordInput = document.getElementById("signup_password");
                const confirmPasswordInput = document.getElementById("signup_confirm_password");

                const body = {
                    login: loginInput.value,
                    email: emailInput.value,
                    password: passwordInput.value,
                    PasswordConfirm: confirmPasswordInput.value,
                }

                sendRequest('POST', requestURL, body)
                    .then(data => {
                        if (data.success) {
                            cleaningAndClosingForm({
                                login: loginInput,
                                email: emailInput,
                                password: passwordInput,
                                passwordConfirm: confirmPasswordInput
                            }, errorContainer);
                            console.log('Успешная регистрация:', data);
                            location.reload();
                        } else {
                            displayErrors([data.error || 'Ошибка регистрации'], errorContainer);
                        }
                    })
                    .catch(err => {
                        if (Array.isArray(err)) {
                            displayErrors(err, errorContainer);
                        } else if (err.errors) {
                            displayErrors(err.errors, errorContainer);
                        } else {
                            displayErrors([err.error || 'Ошибка сервера'], errorContainer);
                        }
                        console.log('Ошибка регистрации:', err);
                    });
            });
        }

        function cleaningAndClosingForm(form, errorContainer) {
            errorContainer.innerHTML = "";
            for (const key in form) {
                if (form.hasOwnProperty(key) && form[key]) {
                    form[key].value = "";
                }
            }
            hiddenOpen_CloseClick();
        }

        function displayErrors(errors, errorContainer) {
            errorContainer.innerHTML = '';
            if (Array.isArray(errors)) {
                errors.forEach(error => {
                    const errorMessage = document.createElement('div');
                    errorMessage.classList.add('error');
                    errorMessage.textContent = error;
                    errorContainer.appendChild(errorMessage);
                });
            } else if (typeof errors === 'string') {
                const errorMessage = document.createElement('div');
                errorMessage.classList.add('error');
                errorMessage.textContent = errors;
                errorContainer.appendChild(errorMessage);
            }
        }

        function sendRequest(method, url, body = null) {
            const headers = {
                'Content-Type': 'application/json'
            }

            return fetch(url, {
                method: method,
                body: JSON.stringify(body),
                headers: headers
            }).then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw errorData;
                    });
                }
                return response.json();
            });
        }

        function hiddenOpen_CloseClick() {
            const modal = document.querySelector(".container-login-registration");
            if (modal) {
                modal.style.display = "none";
                document.body.style.overflow = '';
                document.body.style.position = '';
                document.body.style.width = '';
            }
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

        // Для мобилок
        const sideMenuButton = document.getElementById("side-menu-button-click-to-hide");
        if (sideMenuButton) {
            sideMenuButton.addEventListener("click", toggleModal);
        }

        document.addEventListener('click', function (e) {
            if (e.target.id === 'side-menu-button-click-to-hide' ||
                e.target.closest('#side-menu-button-click-to-hide')) {
                e.preventDefault();
                console.log('Delegated handler caught the click!');
                toggleModal();
            }
        });
    });