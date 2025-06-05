import { API_URL_PREFIX } from '$env/static/private';

export async function POST({ locals, request, fetch }) {
	const join = await fetch(`http://${API_URL_PREFIX}/joingame`, {
		method: 'POST',
		body: JSON.stringify({
			SessionToken: locals.sessionID
		}),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return new Response(JSON.stringify(join), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
