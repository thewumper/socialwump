import { redirect } from '@sveltejs/kit';

export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const users = await event.fetch('http://wumpapi:8080/graph'); // Replace with your actual data source

	if (users.status === 204) {
		return redirect(303, '/waiting');
	}

	const json = await users.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
