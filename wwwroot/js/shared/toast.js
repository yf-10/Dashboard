/**
 * BootstrapのトーストUIを簡単に表示するためのクラス
 * シングルトンパターンでインスタンスを管理し、info/errorなどの定型トーストも提供する
 * 
 * 使用例:
 *   Toast.info('成功しました')
 *   Toast.error('エラーが発生しました')
 *   Toast.show({ iconHtml: '<i class="fa ..."></i>', title: 'カスタム', titleClass: 'text-primary', bodyText: '本文' })
 */
class Toast {
  /**
   * Toastクラスのインスタンスを生成する
   * @throws {Error} 'commonToast element not found' - トーストのDOM要素が見つからない場合
   * @throws {Error} 'Toast inner elements not found' - 必要な内部要素が見つからない場合
   */
  constructor() {
    this.toastEl = document.getElementById('commonToast')
    if (!this.toastEl) {
      throw new Error('commonToast element not found')
    }
    this.toast = new bootstrap.Toast(this.toastEl, {
      animation: true,
      autohide: true,
      delay: 5000
    })
    this.headerTextEl = this.toastEl.querySelector('.toast-header-text')
    this.headerTimeEl = this.toastEl.querySelector('.toast-header-time')
    this.bodyTextEl = this.toastEl.querySelector('.toast-body-text')
    if (!this.headerTextEl || !this.headerTimeEl || !this.bodyTextEl) {
      throw new Error('Toast inner elements not found')
    }
  }

  /**
   * トーストを表示する
   * @param {Object} options - 表示オプション
   * @param {string} [options.iconHtml] - タイトル左側に表示するHTML（例: アイコン）
   * @param {string} options.title - タイトル文字列
   * @param {string} [options.titleClass] - タイトルに付与するクラス（色など）
   * @param {string} options.bodyText - 本文テキスト
   * @description
   *   iconHtmlはHTMLとして挿入される
   *   titleはspan要素内に挿入され、titleClassで色などを指定できる
   *   bodyTextはtextContentで挿入されるためXSSリスクが低い
   *   トーストの右上には現在時刻（時:分）が表示される
   */
  show({ iconHtml, title, titleClass = '', bodyText }) {
    const now = new Date()
    this.headerTextEl.innerHTML =
      (iconHtml ? iconHtml : '') +
      `<span class="${titleClass}">${title}</span>`
    this.headerTimeEl.textContent = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    this.bodyTextEl.textContent = bodyText
    this.toast.show()
  }

  /**
   * 情報トーストを表示する（緑色アイコン・INFOタイトル）
   * @param {string} message - 本文テキスト
   */
  static info(message) {
    Toast.instance().show({
      iconHtml: '<i class="fa-solid fa-circle-info text-success me-2"></i>',
      title: 'INFO',
      titleClass: 'text-success',
      bodyText: message
    })
  }

  /**
   * エラートーストを表示する（赤色アイコン・ERRORタイトル）
   * @param {string} message - 本文テキスト
   */
  static error(message) {
    Toast.instance().show({
      iconHtml: '<i class="fa-solid fa-circle-exclamation text-danger me-2"></i>',
      title: 'ERROR',
      titleClass: 'text-danger',
      bodyText: message
    })
  }

  /**
   * 任意のオプションでトーストを表示する
   * @param {Object} options - showメソッドと同じ
   */
  static show(options) {
    Toast.instance().show(options)
  }

  /**
   * Toastクラスのシングルトンインスタンスを取得する
   * @returns {Toast}
   */
  static instance() {
    if (!Toast._instance) {
      Toast._instance = new Toast()
    }
    return Toast._instance
  }
}

// グローバル関数
/**
 * 情報トーストを表示する（グローバル関数）
 * @function
 * @param {string} message - 本文テキスト
 */
window.ShowInfoToast = Toast.info
/**
 * エラートーストを表示する（グローバル関数）
 * @function
 * @param {string} message - 本文テキスト
 */
window.ShowErrorToast = Toast.error
/**
 * 任意のオプションでトーストを表示する（グローバル関数）
 * @function
 * @param {Object} options - showメソッドと同じ
 */
window.ShowToast = Toast.show
