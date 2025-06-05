export async function POST({ locals, request, fetch }) {
	const join = await fetch('http://wumpapi:8080/joingame', {
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
