import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { IRecommendationHistoryResponse } from "@/services/recommendationanalysis.service";
import { toReadableDate } from "@/util/functions/dateformatter";
import { consume } from "@lit/context";
import Chart from 'chart.js/auto';
import { LitElement, PropertyValueMap, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
  
export const ContentElementTag = "urltracker-history-chart";
  
@customElement(ContentElementTag)
export class UrlTrackerHistoryChart extends LitElement {
  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @property({ attribute: false, type: Object })
  private history!: IRecommendationHistoryResponse;

  private chartRef: Ref<HTMLCanvasElement> = createRef(); 

  private init() {
    const data = this.history.dailyOccurances;

    new Chart(
      this.chartRef.value!,
      {
        type: 'bar',
        options: {
          maintainAspectRatio: false,
          scales: {
            x: {
              display: false
            },
            y: {
              display: false
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
          labels: data.map(row => row.dateTime),
          datasets: [
            {
              label: 'Occurances per day',
              data: data.map(row => row.occurances),
              backgroundColor: '#1B264F',
              minBarLength: 0.5
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
      <div class="history-chart">
        <div class="chart-container">
          <canvas ${ref(this.chartRef)}></canvas>
        </div>
        <div class="history-chart-legend">
          <div class="item">
            <dt>First occurrance</dt>
            <dd>${toReadableDate(new Date(this.history.firstOccurance))}</dd>
          </div>
          <div class="item">
            <dt>Last occurrance</dt>
            <dd>${toReadableDate(new Date(this.history.lastOccurance))}</dd>
          </div>
          <div class="item">
            <dt>Average per day</dt>
            <dd>${this.history.averagePerDay}</dd>
          </div>
          <div class="item">
            <dt>Trend</dt>
            <dd>${this.history.trend}</dd>
          </div>
        </div>
      </div>
    `;
  }

  static styles = css`
    .chart-container {
      width: 100%;
      height: 100px;
    }

    .history-chart-legend{
      margin-top: 1rem;
      margin-bottom: 3rem;
    }

    .item {
        display: grid;
        grid-template-columns: 160px 1fr;
    }

    dt {
      font-size: 12px;
      font-weight: 400;
      line-height: 15px;
    }

    dd {
      font-size: 12px;
      font-weight: 400;
      line-height: 15px;
      color: #68676B;
    }
  `;
}
  