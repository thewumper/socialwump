import { API_URL_PREFIX } from '$env/static/private';

export async function POST({ locals, request, fetch }) {
	// Logic to fetch users (e.g., from a database)

	const join = await fetch(`http://${API_URL_PREFIX}/startgame`, {
		method: 'POST',
		body: JSON.stringify({
			SessionToken: locals.sessionID
		}),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return new Response(JSON.stringify({}), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
