<template>

<div class="rel">
	<lazy-component>
		<v-card class="mx-auto" :ripple="false" :class="{ 'on-hover': hover, 'active': module.cardActive }" @click="this.focus()">
			<header class="pkg-header" :class="typeClass">
				<v-card-title>{{ module.title }}</v-card-title>
				<v-card-subtitle>
					<span class="pkg-type">{{module.type}}</span>
					-
					<span class="pkg-ver">v{{ module.latest }}</span>
				</v-card-subtitle>
			</header>
			<main>
				<v-card-text style="padding-top: 10px;">
					<h4>Author(s): {{ module.author }}</h4>
					<p class="desc" v-html="module.description"></p>
				</v-card-text>
			</main>
			<footer>
				<v-card-text class="languages">
					<v-chip class="languageChip" v-for="language in languages" :key="language">
						<v-icon size="1.5em" left>mdi-translate</v-icon>
						{{ language }}
					</v-chip>
				</v-card-text>
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
	</lazy-component>
</div>

</template>

<script>
import { getByTag } from 'locale-codes';

export default {
	name: "Module",
	props: {
		modules: Array,
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
	methods: {
		
	}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
$radius: 10px;
$trans-dur: .5s;
$size-trans: $trans-dur height, $trans-dur width;

@mixin fancy-scroll {
	&::-webkit-scrollbar { 
		width: 4px;
		height: 4px;

		&-track, &-track-piece {
			background-color: var(--v-secondary-base);
		}
		&-thumb {
			background-color: var(--v-primary-base);
		}
	}
}
.rel {
	position: relative;
	height: 13em;

	.v-card {
		position: absolute;
		top: 0;
		right: 0;
		width: 100%;
		max-width: 55ch;
		height: 13em;
		box-shadow: none;
		transition: .2s box-shadow;
		background-color: var(--v-accent-base);
		border-radius: $radius;
		z-index: 0;
		transition: $trans-dur box-shadow,  $size-trans, 0s $trans-dur z-index;

		

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

			.v-card {
				&__title {
					font-size: 1.1em;
					margin-top: 0;
					line-height: 1em;
					overflow: hidden;
					text-overflow: ellipsis;
					white-space: nowrap;
					width: calc(100% - 30px);
					display: inline-block;
				}
				&__subtitle {
					line-height: 1em;
					margin: 0 {
						top: -1.3em;
					}
				}
			}

			.pkg-type {
				margin-left: -2px;
				display: inline-block;
				padding: 6px;
				background-color: var(--color);
				border-radius: $radius;
			}
			
			&::after {
				content: " ";
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
			transition: $size-trans;

			.v-card__text {
				height: 100%;
			}
			.desc {
				overflow-y: hidden;
				height: 100%;
				position: relative;

				@include fancy-scroll;

				
			}
			&::after {
				content: " ";
				position: absolute;
				bottom: 1em;
				right: 0;
				width: 100%;
				height: 2em;
				background-image: linear-gradient(to bottom, transparent, var(--v-accent-base));
			}
		}
		footer {
			height: 0em;
			overflow: hidden;
			transition: $size-trans;

			.languages {
				height: 4em;
				width: 96%;
				margin-left: 2%;
				white-space: nowrap;
				overflow: hidden {
					x: auto;
				}
				@include fancy-scroll;
			}
		}
		&:focus,
		&:focus-within {
			width: 55ch;
			height: 26.5em;
			z-index: 1;
			box-shadow: 0 3px 5px 2px #000000a6;
			transition: $trans-dur box-shadow, $size-trans, 0s 0s z-index;

			footer {
				height: 6em;
			}
			main {
				height: 15em;
				.desc {
					height: 100%;
					overflow-y: scroll;
					
				}
				&::after {
					background: none;
				}
			}
		}
		&:hover {
			box-shadow: 0 3px 10px 2px #000000a6;
		}
		&--link:before {
			z-index: 1;
		}
		&--link:focus:before {
			opacity: .08;
		}
	}
	
}
span.languageChip {
	margin-right: 5px;
}
</style>
