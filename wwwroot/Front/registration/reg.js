document.getElementById('registerButton').addEventListener('click', async function (event) {
    event.preventDefault();

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const passRepeat = document.getElementById('passwordConfirmation').value;

    // Очистка сообщения перед валидацией
    document.getElementById('message').innerText = '';

    // Проверяем, существует ли пользователь
    const userExists = await checkUserExists(name, email);
    if (userExists) {
        document.getElementById('message').innerText = 'Пользователь с таким именем или email уже существует.';
        document.getElementById('name').value = '';
        document.getElementById('email').value = '';
        return; // Завершаем выполнение, если пользователь существует
    }

    // Валидация имени пользователя
    if (name.length < 5) {
        document.getElementById('message').innerText = 'Имя пользователя должно быть больше 5 символов.';
        document.getElementById('name').value = '';
        return; // Завершаем выполнение, если имя пользователя некорректно
    }

    // Валидация email
    if (!email.endsWith('@gmail.com')) {
        document.getElementById('message').innerText = 'Email должен содержать "gmail.com".';
        document.getElementById('email').value = '';
        return; // Завершаем выполнение, если email некорректен
    }

    // Валидация пароля
    if (password.length < 6) {
        document.getElementById('message').innerText = 'Пароль должен быть не менее 6 символов.';
        document.getElementById('password').value = '';
        document.getElementById('passwordConfirmation').value = '';
        return; // Завершаем выполнение, если пароль некорректен
    }

    // Проверка на наличие хотя бы одной буквы
    const letterRegex = /[a-zA-Z]/;
    if (!letterRegex.test(password)) {
        document.getElementById('message').innerText = 'Пароль должен содержать хотя бы одну букву.';
        document.getElementById('password').value = '';
        document.getElementById('passwordConfirmation').value = '';
        return; // Завершаем выполнение, если пароль некорректен
    }

    // Проверка на совпадение паролей
    if (password !== passRepeat) {
        document.getElementById('message').innerText = 'Пароли не совпадают.';
        document.getElementById('password').value = '';
        document.getElementById('passwordConfirmation').value = '';
        return; // Завершаем выполнение, если пароли не совпадают
    }


    // Если пользователь не существует, создаем нового
    const user = { name, email, password };

    try {
        const response = await fetch('/auth/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (response.ok) {
            const result = await response.json();
            document.getElementById('message').innerText = `Пользователь ${name} успешно зарегистрирован!`;

            // Очистка полей ввода
            document.getElementById('name').value = '';
            document.getElementById('email').value = '';
            document.getElementById('password').value = '';
            document.getElementById('passwordConfirmation').value = '';
        } else {
            const error = await response.json();
            document.getElementById('message').innerText = `Ошибка: ${error.message}`;
        }
    } catch (error) {
        document.getElementById('message').innerText = `Ошибка: ${error.message}`;
    }
});

async function checkUserExists(name, email) {
    try {
        const response = await fetch(`/auth/check?name=${encodeURIComponent(name)}&email=${encodeURIComponent(email)}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (!response.ok) {
            throw new Error('Ошибка при проверке пользователя');
        }
        const data = await response.json();
        return data.exists; // Предполагаем, что сервер возвращает { exists: true/false }
    } catch (error) {
        console.error(error);
        return false;
    }
}