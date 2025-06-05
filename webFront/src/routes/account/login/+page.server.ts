import { redirect } from '@sveltejs/kit';
import { fail } from '@sveltejs/kit';

export const actions = {
	default: async ({ cookies, request }) => {
		const data = await request.formData();
		const username = data.get('uname');
		const code = data.get('code');
		const email = data.get('email');

		const authStatus = await fetch('http://wumpapi:8080/login', {
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

		if (authStatus.status !== 200) {
			return fail(400, { email, message: body.message });
		}

		// TODO! Don't ruin my security
		cookies.set('sessionID', body.sessionToken, { path: '/', secure: false });

		return redirect(303, '/');
	}
};
