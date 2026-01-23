import { toReadableDate } from '@/util/functions/dateformatter';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import Chart from 'chart.js/auto';
import { LitElement, PropertyValueMap, css, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import type { RecommendationHistory } from '../../../../../api-client/types.gen';

export const ContentElementTag = 'urltracker-history-chart';

@customElement(ContentElementTag)
export class UrlTrackerHistoryChart extends UmbElementMixin(LitElement) {
  @property({ attribute: false, type: Object })
  private history!: RecommendationHistory;

  private chartRef: Ref<HTMLCanvasElement> = createRef();

  private async init() {
    const data = this.history.dailyOccurances;

    const label = this.localize.term('urlTrackerChart_occurances-per-day');

    new Chart(this.chartRef.value!, {
      type: 'bar',
      options: {
        maintainAspectRatio: false,
        scales: {
          x: {
            display: false,
          },
          y: {
            display: false,
          },
        },
        plugins: {
          legend: {
            display: false,
          },
          tooltip: {
            enabled: false,
          },
        },
      },
      data: {
        labels: data.map((row) => row.dateTime),
        datasets: [
          {
            label: label,
            data: data.map((row) => row.occurances),
            backgroundColor: '#1B264F',
            minBarLength: 0.5,
          },
        ],
      },
    });
  }

  protected firstUpdated(_changedProperties: PropertyValueMap<unknown> | Map<PropertyKey, unknown>): void {
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
            <dt>
              <umb-localize key="urlTrackerChart_first-occurance">First occurance</umb-localize>
            </dt>
            <dd>${toReadableDate(new Date(this.history.firstOccurance))}</dd>
          </div>
          <div class="item">
            <dt><umb-localize key="urlTrackerChart_last-occurance">Last occurance</umb-localize></dt>
            <dd>${toReadableDate(new Date(this.history.lastOccurance))}</dd>
          </div>
          <div class="item">
            <dt>
              <umb-localize key="urlTrackerChart_average-per-day">Average per day</umb-localize>
            </dt>
            <dd>${this.history.averagePerDay}</dd>
          </div>
          <div class="item">
            <dt>
              <umb-localize key="urlTrackerChart_trend">Trend</umb-localize>
            </dt>
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

    .history-chart-legend {
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
      color: #68676b;
    }
  `;
}
