export async function POST({ locals, request, fetch }) {
	// Logic to fetch users (e.g., from a database)

	const join = await fetch('http://wumpapi:8080/startgame', {
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
