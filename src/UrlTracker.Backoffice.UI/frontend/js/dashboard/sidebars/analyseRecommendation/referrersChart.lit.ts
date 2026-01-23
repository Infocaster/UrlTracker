import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { Chart } from 'chart.js';
import { LitElement, PropertyValueMap, css, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import type { ReferrerResponse } from '../../../../../api-client/types.gen';

export const ContentElementTag = 'urltracker-referrers-chart';

@customElement(ContentElementTag)
export class UrlTrackerReferrersChart extends UmbElementMixin(LitElement) {
  @property({ type: Array })
  private referrers!: ReferrerResponse[];

  private chartRef: Ref<HTMLCanvasElement> = createRef();

  private truncate = (str: string, n: number = 75) => {
    return str.length > n ? str.slice(0, n - 1) + '&hellip;' : str;
  };

  private async init() {
    const data = this.referrers;

    const label = this.localize.term('urlTrackerChart_occurances-per-day');

    const chart = new Chart(this.chartRef.value!, {
      type: 'bar',
      options: {
        maintainAspectRatio: false,
        indexAxis: 'y',
        scales: {
          x: {
            display: false,
            reverse: true,
          },
          y: {
            position: 'right',
            grid: {
              display: false,
            },
            border: {
              display: false,
            },
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
        labels: data.map((row) => this.truncate(`${row.referrerOccurances} - ${row.referrerUrl}`)),
        datasets: [
          {
            label: label,
            data: data.map((row) => row.referrerOccurances),
            backgroundColor: '#1B264F',
            maxBarThickness: 25,
          },
        ],
      },
    });

    (chart.canvas.parentNode as HTMLDivElement).style.height = `${data.length * 20 + 20}px`;
  }

  protected firstUpdated(_changedProperties: PropertyValueMap<unknown> | Map<PropertyKey, unknown>): void {
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
