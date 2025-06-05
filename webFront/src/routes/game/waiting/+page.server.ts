import { redirect } from '@sveltejs/kit';

export async function load({ locals, fetch }) {
	const users = await fetch('http://wumpapi:8080/gamestate'); // Replace with your actual data source

	if ((await users.json()) === 'Active') {
		return redirect(303, '/');
	}

	return {
		user: locals.user
	};
}
