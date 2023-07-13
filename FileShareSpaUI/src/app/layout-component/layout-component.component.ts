import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-layout-component',
  templateUrl: './layout-component.component.html',
  styleUrls: ['./layout-component.component.css']
})
export class LayoutComponentComponent implements OnInit {
  @ViewChild('mainMenu') MainMenu: ElementRef;
  collapseMenu = false;
  constructor() { }
  @HostListener('window:resize', ['$event'])
  onResize(event) {
    if (window.innerWidth < 768)
      this.collapseMenu = true;
    else
      this.collapseMenu = false;
  }
  ngOnInit(): void {
    console.log(window.innerWidth)
  }
  MenuHandler()
  {
    console.log(this.MainMenu)
  }
  activeMenuHandler(activeName)
  {
    this.MainMenu.nativeElement.childNodes.forEach(obj => {
      obj.childNodes.forEach(obj2 => {
        if (obj.childNodes[0].innerText.trim() != activeName)
          obj2.classList.remove('active-menu');
        else
          obj2.classList.add('active-menu');
      });
    });
  }
}
