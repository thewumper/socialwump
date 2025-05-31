import { fail } from '@sveltejs/kit';

export const actions = {
	default: async ({ cookies, request }) => {
		const data = await request.formData();
		const username = data.get('uname');
		const code = data.get('code');
		const email = data.get('email');

		const authStatus = await fetch('http://127.0.0.1:8080/createaccount', {
			method: 'POST',
			body: JSON.stringify({
				Username: username,
				Email: email,
				password: code
			}),
			headers: {
				'Content-Type': 'application/json'
			}
		});

		console.log(authStatus);

		if (authStatus.status === 409) {
			return fail(400, { email, message: 'Email or usename already exists' });
		}

		if (authStatus.status !== 201) {
			return fail(400, { email, message: authStatus.statusText });
		}

		const loginRequest = await fetch('http://127.0.0.1:8080/login', {
			method: 'POST',
			body: JSON.stringify({
				Username: username,
				Email: email,
				Password: code
			}),
			headers: {
				'Content-Type': 'application/json'
			}
		});

		if (loginRequest.status !== 200) {
			return fail(400, { email, message: 'Failed to log you in for some ungodly reason' });
		}

		console.log(authStatus);

		return 'Hi';
	}
};
