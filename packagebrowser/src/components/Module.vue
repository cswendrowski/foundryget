<template>

<div class="rel">
	<!-- add some kind of higher box so loader loads before the module loading gets shown -->
	<!-- <v-lazy>
	<v-progress-circular v-if="!isActive" indeterminate size="64" />
	</v-lazy> -->
	<v-lazy
		transition="fade-transition"
		v-model="isActive"
	>
		<div v-if="isActive">
			<module-overlay
				v-bind:overlayState="overlay"
				v-on:click="overlay = $event"
			/>
			<v-card class="mx-auto" :style="cssVars" :ripple="false" @click="()=>{getManifest()}">
				<header class="pkg-header" :class="typeClass">
					<v-card-title>{{ module.title }}</v-card-title>
					<v-card-subtitle>
						<span class="pkg-type">{{module.type}}</span>
						-
						<span class="pkg-ver">v{{ module.latest }}</span>
						<span class="popularity">
							<progress-ring :radius="25" :progress="installs" :stroke="3"/>
							<label>Users</label>
							<span>{{ popularity }}%</span>
						</span>
					</v-card-subtitle>
				</header>
				<main>
					<v-card-text style="padding-top: 10px;">
						<h4 class="author">
							<label>Author(s): </label>
							<span>{{ module.author }}</span>
						</h4>
						<p class="desc" v-html="module.description"></p>
					</v-card-text>
				</main>

					<footer>
						<resize-sensor @resize="footerResizeHandler"></resize-sensor>
						<v-card-text class="languages">
							<v-chip class="languageChip" v-for="language in languages" :key="language">
								<v-icon size="1.5em" left>mdi-translate</v-icon>
								{{ language }}
							</v-chip>
						</v-card-text>
						<div>
							<v-card-text class="dependencies" v-if="hasDependencies">
								<strong>Dependencies:</strong>
								<span v-for="dependency in manifest.dependencies" :key="dependency">
									{{ dependency.name }}
								</span>
							</v-card-text>
						</div>
						<v-card-text class="compatibility">
							<strong>Compatible Foundry Versions:</strong>
							<span>v{{ manifest.minimumCoreVersion }} - v{{ manifest.compatibleCoreVersion }}</span>
						</v-card-text>
						<div class="cardButtonDiv">
							<v-btn text v-bind:href="module.url" target="_blank">
								Project
							</v-btn>
							<v-btn text v-bind:href="foundryPackageUrl" target="_blank">
								Package
							</v-btn>
							<v-btn text v-bind:href="module.manifest" target="_blank">
								Manifest
							</v-btn>
							<v-btn text @click="overlay = !overlay">
								Extra Info
							</v-btn>
						</div>
					</footer>
			</v-card>
		</div>
	</v-lazy>
</div>

</template>

<script>
import axios from "axios";
import ProgressRing from "./ProgressRing.vue";
import ResizeSensor from "vue-resize-sensor";
import ModuleOverlay from "./ModuleOverlay.vue";

export default {
	name: "Module",
	components: {
		ProgressRing,
		ResizeSensor,
		ModuleOverlay
	},
	props: {
		modules: Array,
		module: Object
	},

	data: () => ({
		isActive: false,
		show: false,
		loader: null,
		loading: false,
		manifest: {
			minimumCoreVersion: "?.?.?",
			compatibleCoreVersion: "?.?.?",
			dependencies: []
		},
		manifestLoaded: false,
		hasDependencies: false,
		footerHeight: 0,
		overlay: false,
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
				"system": "typ-system",
				"world": "typ-world"
			}[this.module.type] || "typ-none";
		},
		languages() {
			let languages = [];
			this.module.languages.forEach(language => {
				let languageTag = this.$func.getLanguageByTag(language);
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
		},
		installs() {
			return Number(this.module.installs);
		},
		popularity() {
			const inst = Number(this.module.installs);
			let pop = inst > 1 ? Math.round(inst) : "<1";
			return pop;
		},
		cssVars() {
			return {
				'--footer-height': this.footerHeight + 'px'
			}
		},
	},
	methods: {

		getManifest() {
			if (this.manifestLoaded) return;
			this.manifestLoaded = true;
			axios
				.get(
					"https://forge-vtt.com/api/bazaar/manifest/" +
					this.module.name
				)
				.then(response => {
					this.manifest = response.data;
					if (response.data?.dependencies && response.data.dependencies?.length !== 0) this.hasDependencies = true;
				})
				.catch(() => {
					axios
						.get(
							"https://cors-anywhere.herokuapp.com/" +
							this.module.manifest
						)
						.then(response => {
							this.manifest = response.data;
							if (response.data?.dependencies && response.data.dependencies?.length !== 0) this.hasDependencies = true;
						})
				})
		},

		footerResizeHandler(size) {
			this.footerHeight = size.height;
		}

	}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
$radius: 10px;
$trans-dur: .5s;
$size-trans: $trans-dur height, $trans-dur width;

@keyframes open {
	0% {
		z-index: 0;
	}
	1% {
		z-index: 2;
	}
	99% {
		z-index: 2;
	}
	100% {
		z-index: 1;
	}
}
@keyframes close {
	0% {
		z-index: 1;
	}
	99% {
		z-index: 1;
	}
	100% {
		z-index: 0;
	}
}

@keyframes scroll-fade-in {
	from {height: 0;}
	to {height: 6px;}
}

@mixin fancy-scroll {
	&::-webkit-scrollbar {
		width: 6px;
		height: 6px;
		border-radius: 3px;

		&-track, &-track-piece {
			border-radius: 3px;
			background-color: var(--v-secondary-base);
		}
		&-thumb {
			border-radius: 3px;
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
		animation: $trans-dur close;
		transition: $trans-dur box-shadow,  $size-trans;

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
					width: calc(100% - 70px);
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

			.popularity {
				position: absolute;
				right: 40px;
				top: calc(50% - 1em);
				display: inline-block;
				width: 44px;
				text-align: center;
				height: 44px;
				background-color: var(--v-secondary-base);
				border-radius: 50%;
				svg {
					position: absolute;
					top: -3.5px;
					left: -3.5px;
				}
				label {
					font-size: .8em;
					color: var(--v-accent-lighten3);
					line-height: 1em;
					position: absolute;
					top: -1.2em;
					left: 0;
					width: 100%;
				}
				span {
					font-size: .9em;
					line-height: 1em;
					position: absolute;
					top: 1.2em;
					width: 100%;
					left: 0;
				}
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
			.author {
				overflow: hidden;
				text-overflow: ellipsis;
				white-space: nowrap;
				color: var(--v-accent-lighten2);
				border-bottom: 1px dashed var(--v-accent-lighten1);
				margin-bottom: .5em;

				label {
					color: var(--v-accent-lighten3);
				}
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
			max-height: 0em;
			display: flex;
			flex-wrap: wrap;
			overflow: hidden;
			transition: $trans-dur max-height, $trans-dur width;
			background-color: var(--v-secondary-lighten1);
			.v-chip {
				font-size: 1em;
			}
			.v-card__text {
				padding: 0;
				> * {
					margin: 16px;
				}
			}
			.languages {
				max-height: 4em;
				width: 96%;
				margin-left: 2%;
				white-space: nowrap;
				overflow: hidden {
					x: auto;
				}
				@include fancy-scroll;
				.languageChip{
					margin-right: 5px;
				}
			}
			.cardButtonDiv {
				display: flex;
				overflow-x: hidden;
				flex-wrap: nowrap;
			}
			> div {
				width: 100%;
			}
			.dependencies,
			.compatibility {
				width: 96%;
				margin-left: 2%;
				white-space: nowrap;
				overflow: hidden {
					x: auto;
				}
				@include fancy-scroll;
				&::-webkit-scrollbar {
					height: 0;
					animation-name: scroll-fade-in;
					animation-duration: calc(#{$trans-dur} / 2);
					animation-delay: $trans-dur;
				}
			}
		}
		&:focus,
		&:focus-within {
			width: 55ch;
			height: calc(20em + var(--footer-height));
			animation: $trans-dur open;
			z-index: 1;
			box-shadow: 0 3px 5px 2px #000000a6;
			transition: $trans-dur box-shadow, $size-trans;

			footer {
				max-height: unset;
				min-height: 6em;
			}
			main {
				height: 15em;
				margin-bottom: .5em;
				.desc {
					height: 100%;
					overflow-y: scroll;
				}
				.author {
					overflow: initial;
					white-space: normal;
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
</style>
