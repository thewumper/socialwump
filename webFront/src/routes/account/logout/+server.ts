import { redirect } from '@sveltejs/kit';

export async function GET({ cookies }) {
	const currentToken = cookies.get('sessionID');

	if (!currentToken) {
		return redirect(302, '/account/login');
	}

	const loginRequest = await fetch('http://wumpapi:8080/logout', {
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
