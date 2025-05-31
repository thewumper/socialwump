import { redirect } from '@sveltejs/kit';

export const actions = {
	default: async ({ cookies, request }) => {
		const data = await request.formData();
		const username = data.get('uname');
		const code = data.get('code');
		const email = data.get('email');

		const authStatus = await fetch('http://127.0.0.1:8080/login', {
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

		let body = await authStatus.json();

		cookies.set('sessionID', body.sessionToken, { path: '/' });

		return redirect(303, '/');
	}
};
