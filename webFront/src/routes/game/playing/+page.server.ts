import { API_URL_PREFIX } from '$env/static/private';
import { redirect } from '@sveltejs/kit';

function splitByClassType(items) {
	return items.reduce((result, item) => {
		const classKey = item.classType;
		if (!result[classKey]) {
			result[classKey] = [];
		}
		result[classKey].push(item);
		return result;
	}, {});
}
export async function load({ locals, fetch }) {
	const users = await fetch(`http://${API_URL_PREFIX}/gamestate`); // Replace with your actual data source

	if ((await users.json()) === 'Waiting') {
		return redirect(303, '/game/waiting');
	}

	const shopinforesp = await fetch('/game/shop/getitemlist');
	const json = await shopinforesp.json();

	const splitItems = splitByClassType(json);

	let playerJoined = false;
	let player;
	let inGame;
	if (locals.sessionID) {
		const playerInfo = await fetch(`http://${API_URL_PREFIX}/getplayer/`, {
			method: 'POST',
			body: JSON.stringify({
				SessionToken: locals.sessionID
			}),
			headers: {
				'Content-Type': 'application/json'
			}
		});
			console.log(await playerInfo.status)
			console.log(await playerInfo.body)
			if (await playerInfo.body){
				playerJoined = true;
				player = (await playerInfo.json());
				console.log(player)
			}
			else {
				playerJoined = false;
				player = null;
			}
	}

	return {
		playerJoined: playerJoined,
		user: locals.user,
		shopItems: splitItems,
		player: player,
	};
}
