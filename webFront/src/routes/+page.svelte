<script>
	import { onMount } from 'svelte';
	import * as d3 from 'd3';

	let graph;
	let errored = $state(false);
	let currentTransform = d3.zoomIdentity;

	onMount(async () => {
		// Make the callback async
		// Container setup
		let screenWidth = window.innerWidth;
		let screenHeight = window.innerHeight;

		// Select container
		const container = d3.select('#my_dataviz');
		container.selectAll('*').remove();

		// Create SVG
		const svg = container.append('svg').attr('width', '100%').attr('height', '100%');

		const mainGroup = svg.append('g');

		let data;
		try {
			// Load data with promises
			data = await d3.json('/graph');
			if (data == undefined) {
				errored = true;
				return;
			}
		} catch (error) {
			console.error('Error loading data:', error);
			errored = true;
			return;
		}

		const zoom = d3.zoom().on('zoom', (e) => {
			mainGroup.attr('transform', e.transform);
			console.log('zoomeded');
			currentTransform = e.transform;
		});

		const drag = d3.drag().on('start', dragstarted).on('drag', dragged).on('end', dragended);

		// Create links
		const link = mainGroup
			.selectAll('line')
			.data(data.links)
			.join('line')
			.attr('class', 'node')
			.style('stroke', '#aaa');

		var div = d3
			.select('.wrapper')
			.insert('div', '.graphContainer')
			.attr('class', 'tooltip-donut')
			.style('opacity', 0);

		// Create nodes
		const node = mainGroup
			.selectAll('circle')
			.data(data.nodes)
			.join('circle')
			.attr('class', 'node')
			.attr('r', 20)
			.style('fill', '#69b3a2')
			.on('mouseover', function (event) {
				let me = d3.select(this);

				me.transition().duration('50').style('fill', '#00796B');
			})
			.on('mouseout', function (d, i) {
				d3.select(this).transition().duration(50).style('fill', '#69b3a2');

				// div.transition().duration(50).style('opacity', 0);
			})
			.on('click', function (event) {
				let me = d3.select(this);
				div.transition().duration(50).style('opacity', 1);
				let num = me.datum().name;
				div
					.html(num)
					.style('left', event.clientX + 10 + 'px')
					.style('top', event.clientY - 15 + 'px');
			})
			.call(drag);

		svg.call(zoom);

		// Force simulation
		const simulation = d3
			.forceSimulation(data.nodes)
			.force(
				'link',
				d3
					.forceLink()
					.id((d) => d.id)
					.links(data.links)
			)
			.force('charge', d3.forceManyBody().strength(-400))
			.force('center', d3.forceCenter(screenWidth / 2, screenHeight / 2))
			.on('tick', ticked);

		function ticked() {
			link
				.attr('x1', (d) => d.source.x)
				.attr('y1', (d) => d.source.y)
				.attr('x2', (d) => d.target.x)
				.attr('y2', (d) => d.target.y);

			node.attr('cx', (d) => d.x).attr('cy', (d) => d.y);
		}

		function dragstarted(event, d) {
			if (!event.active) simulation.alphaTarget(0.3).restart();
			d.fx = event.x;
			d.fy = event.y;
		}

		function dragged(event, d) {
			d.fx = event.x;
			d.fy = event.y;
		}

		function dragended(event, d) {
			if (!event.active) simulation.alphaTarget(0);
			d.fx = null;
			d.fy = null;
		}
	});
</script>

<div class="wrapper">
	{#if errored}
		<div class="centerStuffPlease">
			<h1 class="errorText">Thigns have exploded :(</h1>
		</div>
	{/if}

	<div id="my_dataviz" class="graphContainer"></div>
</div>

<style>
	.graphContainer {
		width: 100vw;
		height: 100vh;
	}

	.centerStuffPlease {
		position: absolute;
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100vw;
		height: 100vh;
	}

	.errorText {
		font-size: 10rem;
		color: #d32f2f;
	}

	.wrapper {
		position: relative;
		background-color: #212121;
	}

	:global(div.tooltip-donut) {
		position: absolute;
		text-align: center;
		padding: 0.5rem;
		background: #ffffff;
		color: #313639;
		border: 1px solid #313639;
		border-radius: 8px;
		pointer-events: none;
		font-size: 1.3rem;
	}
</style>
