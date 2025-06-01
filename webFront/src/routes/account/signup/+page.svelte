<script>
	import { enhance } from '$app/forms';
	let hasSavedCode = $state(false);
	let modalOpen = $state(false);

	let formelement;

	let { data, form } = $props();

	const loginCode = String(Math.floor(Math.random() * (999999 - 0 + 1))).padStart(6, '0');
</script>

<div class="relative flex h-screen w-screen flex-col items-center justify-center bg-zinc-900">
	{#if modalOpen}
		<div
			class="absolute flex h-screen w-screen items-center justify-center bg-zinc-800 opacity-80"
		></div>
		<div
			class="absolute flex w-5/6 max-w-80 flex-col items-center justify-center gap-4 bg-white bg-zinc-600 p-2 text-left text-zinc-50"
		>
			<h1 class="text-3xl">Login code</h1>
			<p class="">Screenshot or otherwise save this code, this is your account password</p>
			<h1 class="text-center">{loginCode}</h1>
			<button
				form="loginForm"
				class="rounded-sm bg-blue-400 p-2 hover:bg-blue-500"
				onclick={() => {
					hasSavedCode = true;
					modalOpen = false;
					formelement.submit();
				}}>I have saved this code and not lose it</button
			>
		</div>
	{/if}
	<div class="flex w-3/4 max-w-96 flex-col rounded-sm border-2 border-gray-300 p-3 text-zinc-50">
		<h1 class="pb-2 text-left text-4xl font-bold">Sign up</h1>
		<form
			id="loginForm"
			method="post"
			class="flex flex-col gap-2"
			bind:this={formelement}
			use:enhance
		>
			<label class="flex flex-col">
				Email
				<input
					class="bg-zinc-700"
					type="text"
					name="email"
					id="email"
					value={form?.email ? form?.email : ''}
					required
				/>
			</label>
			<label class="flex flex-col">
				Username
				<input class="bg-zinc-700" type="text" name="uname" id="uname" required />
			</label>
			<input type="hidden" name="code" value={loginCode} />
			{#if form?.message}
				<p class="font-semibold text-red-600">{form.message}</p>
			{/if}
			<button
				type="button"
				class="my-2 rounded-sm bg-blue-900 p-2 text-white transition-colors duration-150 hover:bg-blue-800 active:bg-blue-950"
				onclick={() => (modalOpen = true)}>Next</button
			>
		</form>
		<a
			href="/account/login"
			class="pt-2 text-center text-zinc-400 hover:text-zinc-300 active:text-zinc-500">Or login</a
		>
	</div>
</div>
