// ------------------------------------
// Common Toast
// ------------------------------------
class CommonToast {
  constructor() {
    this.toastEl = document.getElementById('commonToast');
    this.toast = new bootstrap.Toast(this.toastEl, {
      animation: true,
      autohide: true,
      delay: 5000
    })
    this.headerTextEl = this.toastEl.querySelector('.toast-header-text');
    this.headerTimeEl = this.toastEl.querySelector('.toast-header-time');
    this.bodyTextEl = this.toastEl.querySelector('.toast-body-text');
  }
  Show(header, body) {
    const now = new Date();
    this.headerTextEl.innerHTML = header;
    this.headerTimeEl.innerHTML = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
    this.bodyTextEl.innerHTML = body;
    this.toast.show();
  }
}

const commonToast = new CommonToast();

function ShowInfoToast(message) {
  const headerHtml = '<i class="fa-solid fa-circle-info text-success me-2"></i><span class="text-success">INFO</span>';
  const bodyHtml = message;
  commonToast.Show(headerHtml, bodyHtml);
}

function ShowErrorToast(message) {
  const headerHtml = '<i class="fa-solid fa-circle-exclamation text-danger me-2"></i><span class="text-danger">ERROR</span>';
  const bodyHtml = message;
  commonToast.Show(headerHtml, bodyHtml);
}
