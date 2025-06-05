export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const items = await event.fetch('http://wumpapi:8080/iteminfo'); // Replace with your actual data source
	const json = await items.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
