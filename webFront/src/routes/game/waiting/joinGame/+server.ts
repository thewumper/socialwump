export async function POST({ locals, request, fetch }) {
	// Logic to fetch users (e.g., from a database)

	const formdata = await request.formData();

	const join = await fetch('http://127.0.0.1:42069/joingame', {
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
