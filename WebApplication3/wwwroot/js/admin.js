// Admin Panel JavaScript
document.addEventListener('DOMContentLoaded', function () {
    initAdminPanel();
});

function initAdminPanel() {
    // Инициализация модальных окон
    initModals();

    // Инициализация форм
    initForms();

    // Инициализация фильтров
    initFilters();
}

function initModals() {
    // Закрытие модальных окон
    document.querySelectorAll('.modal-close, .admin-modal').forEach(element => {
        element.addEventListener('click', function (e) {
            if (e.target === this) {
                closeModal();
            }
        });
    });
}

function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }
}

function closeModal() {
    document.querySelectorAll('.admin-modal').forEach(modal => {
        modal.style.display = 'none';
    });
    document.body.style.overflow = '';
}

function initForms() {
    // Подтверждение опасных действий
    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', function (e) {
            const message = this.getAttribute('data-confirm');
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });
}

function initFilters() {
    // Авто-сабмит форм фильтрации при изменении
    document.querySelectorAll('.filter-form select, .filter-form input').forEach(element => {
        element.addEventListener('change', function () {
            this.closest('form').submit();
        });
    });
}

// Функции для AJAX операций
async function updateUserRole(userId, newRole) {
    if (!confirm('Вы уверены, что хотите изменить роль пользователя?')) {
        return;
    }

    const formData = new FormData();
    formData.append('userId', userId);
    formData.append('newRole', newRole);

    try {
        const response = await fetch('/Admin/UpdateUserRole', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            location.reload();
        } else {
            alert('Ошибка при обновлении роли');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ошибка при обновлении роли');
    }
}