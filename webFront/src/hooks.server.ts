import { redirect } from '@sveltejs/kit';

export const handle = async ({ event, resolve }) => {
	const requestedPath = event.url.pathname;
	const cookies = event.cookies;

	// Auth check will go here
	let currentToken = cookies.get('sessionID');

	if (currentToken) {
		const authStatus = await event.fetch('http://127.0.0.1:8080/validateauth', {
			method: 'POST',
			body: JSON.stringify({
				sessiontoken: currentToken
			}),
			headers: {
				'Content-Type': 'application/json'
			}
		});
		console.log(await authStatus.json());
	} else {
		if (!requestedPath.startsWith('/account')) {
			return redirect(303, 'account/login');
		}
	}

	console.log(requestedPath);

	const response = await resolve(event);

	return response;
};
