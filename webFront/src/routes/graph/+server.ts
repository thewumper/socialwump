export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const users = await event.fetch('http://127.0.0.1:8080/maxWantsADummyBecauseHeIsADummy'); // Replace with your actual data source
	const json = await users.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
