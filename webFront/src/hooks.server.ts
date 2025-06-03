import { error, redirect } from '@sveltejs/kit';

export const handle = async ({ event, resolve }) => {
	const requestedPath = event.url.pathname;
	const cookies = event.cookies;

	// Auth check will go here
	let currentToken = cookies.get('sessionID');

	let authed = false;

	let body;
	let authStatus;
	if (currentToken) {
		authStatus = await event.fetch('http://127.0.0.1:42069/validateauth', {
			method: 'POST',
			body: JSON.stringify({
				sessiontoken: currentToken
			}),
			headers: {
				'Content-Type': 'application/json'
			}
		});
		if (!authStatus || authStatus.status === 500) {
			console.error(authStatus);
			return error(404, 'No response from auth system');
		}

		body = await authStatus?.json();
		authed = true;

		if (!body.success) {
			event.cookies.delete('sessionID', { path: '/' });
			authed = false;
		}
	}

	// Everyone here should be logged in

	if (authed) {
		event.locals.sessionID = currentToken;
		event.locals.user = body.user;
	}

	const response = await resolve(event);
	return response;
};
