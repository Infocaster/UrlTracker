import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { IRecommendationReferrerResponse } from "@/services/recommendationanalysis.service";
import { consume } from "@lit/context";
import { Chart } from "chart.js";
import { LitElement, PropertyValueMap, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
    
export const ContentElementTag = "urltracker-referrers-chart";

@customElement(ContentElementTag)
export class UrlTrackerReferrersChart extends LitElement {
  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @property({ type: Array })
  private referrers!: IRecommendationReferrerResponse;

  private chartRef: Ref<HTMLCanvasElement> = createRef(); 

  private init() {
    const data = this.referrers;

    new Chart(
      this.chartRef.value!,
      {
        type: 'bar',
        options: {
          maintainAspectRatio: false,
          indexAxis: 'y',
          scales: {
            x: {
              display: false
            },
            y: {
              position: 'right',
              grid: {
                display: false
              },
              border: {
                display: false
              }
            }
          },
          plugins: {
            legend: {
              display: false
            },
            tooltip: {
              enabled: false
            }
          }
        },
        data: {
          labels: data.map(row => `${row.ReferrerOccurances}     ${row.ReferrerUrl}`),
          datasets: [
            {
              label: 'Occurances per day',
              data: data.map(row => row.ReferrerOccurances),
              backgroundColor: '#1B264F',
              maxBarThickness: 20
            }
          ]
        }
      }
    );
  }

  protected firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
    super.firstUpdated(_changedProperties);
    this.init();
  }

  protected render() {
    return html`
      <div class="chart-container">
        <canvas ${ref(this.chartRef)}></canvas>
      </div>
    `;
  }

  static styles = css`
    .chart-container {
      width: 100%;
      height: 100px;
    }
  `;
}
    