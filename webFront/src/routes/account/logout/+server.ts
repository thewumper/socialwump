import { API_URL_PREFIX } from '$env/static/private';
import { redirect } from '@sveltejs/kit';

export async function GET({ cookies }) {
	const currentToken = cookies.get('sessionID');

	if (!currentToken) {
		return redirect(302, '/account/login');
	}

	const loginRequest = await fetch(`http://${API_URL_PREFIX}/logout`, {
		method: 'POST',
		body: JSON.stringify({
			SessionToken: currentToken
		}),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return redirect(302, '/account/login');
}
