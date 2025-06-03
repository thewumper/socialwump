import { redirect } from '@sveltejs/kit';

export async function GET({ fetch }) {
	// Logic to fetch users (e.g., from a database)

	const resp2 = await fetch('/game/waiting/getGameState');
	const json2 = await resp2.json();

	let gameStarted = false;
	if (json2 === 'Active') {
		gameStarted = true;
	}

	console.log('HI');

	if (gameStarted) {
		return redirect(303, '/game/playing');
	} else {
		return redirect(303, '/account/waiting');
	}
}
