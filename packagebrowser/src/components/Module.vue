<template>
<lazy-component>
	<div class="rel">
		<v-hover
			v-slot:default="{ hover }"
			>
			<v-card 
				class="mx-auto"
				:class="{ 'on-hover': hover }"
				>
				<header class="pkg-header" :class="typeClass">
					<v-card-title>{{ module.title }}</v-card-title>
					<v-card-subtitle>{{module.type}} - v{{ module.latest }}</v-card-subtitle>
					<figure class="ribbon"></figure>
				</header>
					<v-card-text style="padding-top: 10px;">
						<h4>Author(s): {{ module.author }}</h4>
						<p class="desc" v-html="module.description"></p>
						<!--<p class="long-desc" v-html="module.longDescription"></p>-->
					</v-card-text>
				<v-card-text v-if="hasLanguages">
					<v-chip class="languageChip" v-for="language in languages" :key="language">
						<v-icon size="1.5em" left>mdi-translate</v-icon>
						{{ language }}
					</v-chip>
				</v-card-text>
				<v-card-actions>
					<v-btn
						text
						v-bind:href="module.url"
						target="_blank"
					>Project</v-btn>

					<v-spacer></v-spacer>
					
					<v-btn
						text
						v-bind:href="foundryPackageUrl"
						target="_blank"
					>Package</v-btn>

					<v-spacer></v-spacer>

						<v-btn
							icon
							@click="show = !show"
						>
							<v-icon>{{ show ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
						</v-btn>
				</v-card-actions>

				<v-expand-transition>
					<div v-show="show">
						<v-divider></v-divider>

						<v-btn
							text
							v-bind:href="module.manifest"
							target="_blank"
						>Manifest</v-btn>

						<!-- <v-btn
							:loading="loading"
							:disabled="loading"
							color="blue-grey"
							class="ma-2 white--text"
							@click="loader = 'loading'"
							>
							ZIP
							<v-icon right dark>mdi-cloud-download</v-icon>
						</v-btn> -->

						<v-card-text>

						<!-- 
							not in forge api, should probably lazyload on expansion

							<b>Compatible Foundry Versions: </b><span>v{{ module.minimumCoreVersion }} - v{{ module.compatibleCoreVersion }}</span> 
						-->
						</v-card-text>
					</div>
				</v-expand-transition>
			</v-card>
		</v-hover>
	</div>
</lazy-component>
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
.rel {
	position: relative;
	min-height: 20em;
	.v-card {
		position: absolute;
		top: 0;
		width: 100%;
		min-height: 15em;
		max-height: 35em;
		box-shadow: none;
		transition: .2s box-shadow;
		background-color: var(--v-accent-base);
		
		.pkg-header {
			position: relative;
			background-color: var(--v-primary-base);
			
			.ribbon {
				box-shadow: -2px 0 5px 1px var(--shadow-color);
				position: absolute;
				display: block;
				width: 30px;
				height: calc(100% - 2px);
				background-color: var(--shadow-color);
				top: 1px;
				right: 0px;
			}
		}

		.typ-module {
			--shadow-color: #774d00;
		}
		.typ-system {
			--shadow-color: #338
		}
		.typ-none {
			--shadow-color: #BBB;
		}

		.desc {
			line-height: 1.5em;
			overflow-y: hidden;
			height: 4.5em;
			transition: .5s height;
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
		/*.short-desc {
			display: block;
		}*/

		.v-btn {
			color: #999;
		}
	}
	.v-card.on-hover {
		z-index: 1;
		box-shadow: 0 3px 10px 2px #000000a6;
		.desc {
			overflow-y: scroll;
			height: 15em;
		}
		/*.short-desc {
			display: none;
		}*/
	}
}
h3 {
	margin: 40px 0 0;
}
ul {
	list-style-type: none;
	padding: 0;
}
li {
	display: inline-block;
	margin: 0 10px;
}
a {
	color: #42b983;
}
span.languageChip {
	margin-right: 5px;
}
</style>
