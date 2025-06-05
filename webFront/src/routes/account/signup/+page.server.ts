import { API_URL_PREFIX } from '$env/static/private';
import { fail, redirect } from '@sveltejs/kit';

const { MODE } = import.meta.env;

export const actions = {
	default: async ({ cookies, request }) => {
		const data = await request.formData();
		const username = data.get('uname');
		const code = data.get('code');
		const email = data.get('email');

		const authStatus = await fetch(`http://${API_URL_PREFIX}/createaccount`, {
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

		if (authStatus.status === 409) {
			return fail(400, { email, message: 'Email or usename already exists' });
		}

		if (authStatus.status !== 201) {
			const json = await authStatus.json();
			return fail(400, { email, message: json.message });
		}

		const loginRequest = await fetch(`http://${API_URL_PREFIX}/login`, {
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

		const json = await loginRequest.json();

		// TODO! Don't ruin my security
		cookies.set('sessionID', json.sessionToken, { path: '/', secure: MODE === 'production' });

		return redirect(303, '/');
	}
};
