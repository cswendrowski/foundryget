<template>

<div class="rel">
	<v-hover v-slot:default="{ hover }">
		<v-card class="mx-auto" :class="{ 'on-hover': hover }">
			<header class="pkg-header" :class="typeClass">
				<v-card-title>{{ module.title }}</v-card-title>
				<v-card-subtitle>
					<span class="pkg-type">{{module.type}}</span>
					-
					<span class="pkg-ver">v{{ module.latest }}</span>
				</v-card-subtitle>
				<figure class="ribbon"></figure>
			</header>
			<main>
				<v-card-text style="padding-top: 10px;">
					<h4>Author(s): {{ module.author }}</h4>
					<p class="desc" v-html="module.description"></p>
				</v-card-text>
				<v-card-text v-if="hasLanguages">
					<v-chip class="languageChip" v-for="language in languages" :key="language">
						<v-icon size="1.5em" left>mdi-translate</v-icon>
						{{ language }}
					</v-chip>
				</v-card-text>
			</main>
			<footer>
				<v-btn text v-bind:href="module.url" target="_blank">
					Project
				</v-btn>
				<v-btn text v-bind:href="foundryPackageUrl" target="_blank">
					Package
				</v-btn>
				<v-btn text v-bind:href="module.manifest" target="_blank">
					Manifest
				</v-btn>
				<v-card-text>
				<!-- 
					// not in forge api, should probably lazyload on expansion
					<strong>Compatible Foundry Versions:</strong>
					<span>v{{ module.minimumCoreVersion }} - v{{ module.compatibleCoreVersion }}</span>
				-->
				</v-card-text>
			</footer>
		</v-card>
	</v-hover>
</div>

</template>

<script>
import { getByTag } from 'locale-codes';

export default {
	name: "Module",
	props: {
		module: Object
	},

	data: () => ({
		show: false,
		loader: null,
		loading: false
	}),

	watch: {
		loader () {
			const l = this.loader
			this[l] = !this[l]

			setTimeout(() => (this[l] = false), 3000)

			this.loader = null
		}
	},

	computed: {
		foundryPackageUrl() {
				return `https://foundryvtt.com/packages/${this.module.name}/`;
		},
		typeClass() {
			return {
				"module": "typ-module",
				"system": "typ-system"
			}[this.module.type] || "typ-none";
		},
		languages() {
			let languages = [];
			this.module.languages.forEach(language => {
				let languageTag = getByTag(language.lang)?.local || getByTag(language.lang)?.name || language.name || language.lang;
				if (!languages.includes(languageTag)) {
					languages.push(languageTag);
				}
			})
			return languages;
		},
		hasLanguages() {
			if (this.module.languages && this.module.languages.length !== 0) {
				return true;
			}
			return false;
		}
	},

	
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
$radius: 10px;

.rel {
	position: relative;
	height: 12em;
	margin-bottom: 2em;

	.v-card {
		position: absolute;
		top: 0;
		width: 100%;
		height: 12em;
		box-shadow: none;
		transition: .2s box-shadow;
		background-color: var(--v-accent-base);
		border-radius: $radius;
		--height-trans: .5s height;
		transition: .5s box-shadow,  var(--height-trans);

		.v-btn {
			color: #999;
		}

		.typ-module {
			--color: #774d00;
		}
		.typ-system {
			--color: #338
		}
		.typ-world {
			--color: #151
		}
		.typ-none {
			--color: #BBB;
		}
		header.pkg-header {
			height: 4.5em;
			position: relative;
			background-color: var(--v-primary-base);

			.v-card__title {
				margin-top: 0;
				line-height: 1em;
			}
			.v-card__subtitle {
				margin-top: -1em;
				line-height: 1em;
			}

			.pkg-type {
				margin-left: -2px;
				display: inline-block;
				padding: 6px;
				background-color: var(--color);
				border-radius: $radius;
			}
			
			.ribbon {
				position: absolute;
				display: block;
				width: 30px;
				height: 100%;
				background-color: var(--color);
				top: 0px;
				right: 0px;
				border-radius: 0 $radius 0 0;
			}
		}

		main {
			line-height: 1.5em;
			overflow-y: hidden;
			height: 7.5em;
			transition: var(--height-trans);

			.desc {
				overflow-y: hidden;
				height: 3em;
			}

			.desc::-webkit-scrollbar {
				width: 4px;
			}
			.desc::-webkit-scrollbar-track,
			.desc::-webkit-scrollbar-track-piece {
				background-color: var(--v-secondary-base);
			}
			.desc::-webkit-scrollbar-thumb {
				background-color: var(--v-primary-base);
			}
		}
		footer {
			height: 0em;
			overflow: hidden;
			transition: var(--height-trans);
		}
		&.on-hover {
			z-index: 1;
			box-shadow: 0 3px 10px 2px #000000a6;
			height: 24em;
			footer {
				height: 4.5em;
			}
			main {
				height: 15em;
				.desc {
					height: 100%;
					overflow-y: scroll;
				}
			}
		}
	}
}
span.languageChip {
	margin-right: 5px;
}
</style>
