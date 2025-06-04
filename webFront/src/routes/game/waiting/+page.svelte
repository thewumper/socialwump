<script>
	import { onMount } from 'svelte';
	import { enhance } from '$app/forms';
	import { redirect } from '@sveltejs/kit';

	let playerCount = $state(0);
	let requiredCount = $state(0);
	let joined = $state(false);

	onMount(async () => {
		setInterval(async () => {
			const resp = await fetch('/game/waiting/getPlayerCount');
			const json = await resp.json();

			const resp2 = await fetch('/game/waiting/getGameState');
			const json2 = await resp2.json();

			playerCount = json.players;
			requiredCount = json.requiredPlayers;

			if (json2 === 'Active') {
				window.location.href = '/';
			}
		}, 1000);
	});
</script>

<div class="flex h-dvh w-dvw flex-col items-center justify-center gap-2 bg-zinc-900 text-zinc-50">
	<h1 class="text-3xl">Waiting for players</h1>
	<div class="flex flex-row justify-baseline gap-1">
		<span class="inline-block text-3xl">Currently at:</span>
		<span class="text-4xl font-bold">{playerCount}/{requiredCount}</span>
	</div>

	<form action="/waiting/joinGame" method="POST" use:enhance>
		<button
			type="submit"
			class="rounded-sm bg-zinc-700 p-1.5 hover:bg-zinc-600 active:bg-zinc-800"
			onclick={() => {
				joined = true;
			}}>Join game</button
		>
	</form>
	{#if joined}
		<h1>You're in the game</h1>
	{/if}

	{#if playerCount >= requiredCount}
		<p>Enough players are present, game is starting soon</p>
	{/if}
</div>
