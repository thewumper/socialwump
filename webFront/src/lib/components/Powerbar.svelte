<script lang="ts">
	let { powerlevel, maxhealth, showNumber } = $props();
</script>

<div class="relative h-full overflow-hidden rounded-md border-4 border-gray-600">
	<div
		class="absolute z-10 h-full bg-blue-500 transition-all duration-300 ease-in-out"
		style="width: {(100 * powerlevel) / maxhealth}%"
	></div>
	<div
		class="absolute z-0 h-full w-full bg-slate-800 transition-all duration-300 ease-in-out"
	></div>
	{#if showNumber}
		<span
			class="absolute top-1/2 left-2 z-20 font-mono text-xl text-white"
			style="transform: translate(0,-50%);"
			>{String(powerlevel).padStart(String(maxhealth).length, '0')}/{maxhealth}</span
		>
	{/if}
</div>
