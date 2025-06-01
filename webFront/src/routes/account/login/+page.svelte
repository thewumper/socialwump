<script>
	import { enhance } from '$app/forms';

	let { data, form } = $props();
</script>

<div class="flex h-dvh w-dvw flex-col items-center justify-center">
	<div class="flex w-3/4 max-w-96 flex-col rounded-sm border-2 border-gray-300 p-3">
		<h1 class="pb-2 text-left text-4xl font-bold">Login</h1>
		<form action="" method="post" class="flex flex-col gap-2" use:enhance>
			<label class="flex flex-col">
				Username
				<input type="text" name="uname" id="uname" required />
			</label>
			<label class="flex flex-col">
				Login Code
				<input type="password" name="code" id="code" required />
			</label>
			{#if form?.message}
				<p class="font-semibold text-red-600">{form.message}</p>
			{/if}
			<button type="submit" class="rounded-sm bg-blue-800 p-2 text-white">Submit</button>
		</form>
		<a href="/account/signup" class="pt-2 text-center text-slate-950 hover:text-slate-600"
			>Or sign up</a
		>
	</div>
</div>
