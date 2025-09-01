import { ILocalizationService, localizationServiceContext } from '@/context/localizationservice.context';
import { IRecommendationHistoryResponse } from '@/services/recommendationanalysis.service';
import { toReadableDate } from '@/util/functions/dateformatter';
import { ensureServiceExists } from '@/util/tools/existancecheck';
import { consume } from '@lit/context';
import { Task } from '@lit/task';
import Chart from 'chart.js/auto';
import { LitElement, PropertyValueMap, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';

export const ContentElementTag = 'urltracker-history-chart';

type Translations = {
  firstOccurance: string;
  lastOccurance: string;
  averagePerDay: string;
  trend: string;
};

@customElement(ContentElementTag)
export class UrlTrackerHistoryChart extends LitElement {
  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @property({ attribute: false, type: Object })
  private history!: IRecommendationHistoryResponse;

  @state()
  private translationTaskKey: number = 0;

  private chartRef: Ref<HTMLCanvasElement> = createRef();

  private async init() {
    const data = this.history.dailyOccurances;

    ensureServiceExists(this.localizationService, 'localizationService');

    const label = await this.localizationService.localize('urlTrackerChart_occurances-per-day');

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

  private _translationTask = new Task(this, {
    task: async (): Promise<Partial<Translations>> => {
      const [firstOccurance, lastOccurance, averagePerDay, trend] = await Promise.all([
        this.localizationService?.localize('urlTrackerChart_first-occurance'),
        this.localizationService?.localize('urlTrackerChart_last-occurance'),
        this.localizationService?.localize('urlTrackerChart_average-per-day'),
        this.localizationService?.localize('urlTrackerChart_trend'),
      ]);
      return {
        firstOccurance,
        lastOccurance,
        averagePerDay,
        trend,
      };
    },
    args: () => [this.translationTaskKey],
  });

  protected render() {
    return this._translationTask.render({
      complete: (translations: Partial<Translations>) => {
        return html`
          <div class="history-chart">
            <div class="chart-container">
              <canvas ${ref(this.chartRef)}></canvas>
            </div>
            <div class="history-chart-legend">
              <div class="item">
                <dt>${translations.firstOccurance}</dt>
                <dd>${toReadableDate(new Date(this.history.firstOccurance))}</dd>
              </div>
              <div class="item">
                <dt>${translations.lastOccurance}</dt>
                <dd>${toReadableDate(new Date(this.history.lastOccurance))}</dd>
              </div>
              <div class="item">
                <dt>${translations.averagePerDay}</dt>
                <dd>${this.history.averagePerDay}</dd>
              </div>
              <div class="item">
                <dt>${translations.trend}</dt>
                <dd>${this.history.trend}</dd>
              </div>
            </div>
          </div>
        `;
      },
    });
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
